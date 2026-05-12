using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WindowsFormsApp21
{
    /// <summary>
    /// Handles the socket connection to the Python Facial Expression Detector.
    /// This client runs in a background thread and updates the CurrentEmotion property.
    /// </summary>
    public static class EmotionSocketClient
    {
        private static TcpClient _client;
        private static Thread _receiveThread;
        private static bool _isRunning = true;

        /// <summary>
        /// The most recently received emotion.
        /// </summary>
        public static string CurrentEmotion { get; private set; } = "Neutral";

        /// <summary>
        /// Event triggered when a new emotion is received.
        /// </summary>
        public static event Action<string> OnEmotionReceived;

        private const string Host = "127.0.0.1";
        private const int Port = 65435;

        /// <summary>
        /// Starts the background thread to listen for emotion updates.
        /// </summary>
        public static void Start()
        {
            if (_receiveThread != null && _receiveThread.IsAlive) return;

            _isRunning = true;
            _receiveThread = new Thread(ReceiveDataLoop)
            {
                IsBackground = true,
                Name = "EmotionSocketReceiver"
            };
            _receiveThread.Start();
        }

        /// <summary>
        /// Stops the background thread.
        /// </summary>
        public static void Stop()
        {
            _isRunning = false;
            _client?.Close();
        }

        private static void ReceiveDataLoop()
        {
            while (_isRunning)
            {
                try
                {
                    if (_client == null || !_client.Connected)
                    {
                        _client = new TcpClient();
                        // Try to connect (blocking with timeout)
                        var result = _client.BeginConnect(Host, Port, null, null);
                        var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
                        
                        if (!success)
                        {
                            throw new Exception("Connection timeout");
                        }
                        _client.EndConnect(result);
                        Console.WriteLine("Connected to Emotion Server");
                    }

                    NetworkStream stream = _client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        // Data might contain multiple updates if they came in fast (newline separated)
                        string[] emotions = data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var emotion in emotions)
                        {
                            string trimmedEmotion = emotion.Trim();
                            if (string.IsNullOrEmpty(trimmedEmotion)) continue;

                            if (trimmedEmotion == "exit")
                            {
                                _isRunning = false;
                                break;
                            }

                            if (CurrentEmotion != trimmedEmotion)
                            {
                                CurrentEmotion = trimmedEmotion;
                                OnEmotionReceived?.Invoke(CurrentEmotion);
                            }
                        }
                        
                        if (!_isRunning) break;
                    }
                }
                catch (Exception ex)
                {
                    // Update UI with error for debugging
                    CurrentEmotion = "Status: " + ex.Message;
                    OnEmotionReceived?.Invoke(CurrentEmotion);
                    
                    _client?.Close();
                    _client = null;
                }

                if (_isRunning)
                {
                    Thread.Sleep(1000); // Wait 1 second before retrying connection
                }
            }
        }
    }
}
