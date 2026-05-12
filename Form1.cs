using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp21
{
    public partial class Form1 : Form
    {
        TcpListener listener;
        List<string> userLines = new List<string>();

        bool isListening = false;   // important

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            LoadUsers();
            StartListening();

            // Subscribe to emotion updates
            EmotionSocketClient.OnEmotionReceived += UpdateEmotionLabel;
            this.FormClosing += (s, e) => EmotionSocketClient.OnEmotionReceived -= UpdateEmotionLabel;
            
            // Set initial value
            lblEmotion.Text = "Connecting to Camera...";
        }

        private void UpdateEmotionLabel(string emotion)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            if (lblEmotion.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(UpdateEmotionLabel), emotion);
                return;
            }
            lblEmotion.Text = $"Emotion: {emotion}";
        }

        // Load all lines from users.txt
        void LoadUsers()
        {
            string path = "users.txt";

            if (File.Exists(path))
            {
                foreach (var line in File.ReadAllLines(path))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        userLines.Add(line.Trim());
                }
            }
            else
            {
                MessageBox.Show("users.txt not found");
            }
        }

        // Start TCP listener for Python-sent MAC addresses
        async void StartListening()
        {
            if (isListening) return;   // prevents running twice

            isListening = true;

            listener = new TcpListener(IPAddress.Loopback, 65434);
            listener.Start();

            while (isListening)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync();
                    var stream = client.GetStream();

                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string mac = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    bool found = false;
                    string userName = "";
                    string userGender = "male"; // Default

                    foreach (var line in userLines)
                    {
                        if (line.StartsWith("name", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length >= 6) // Need at least index 5 for gender
                        {
                            string bluetoothMac = parts[4].Trim();

                            if (string.Equals(bluetoothMac, mac, StringComparison.OrdinalIgnoreCase))
                            {
                                found = true;
                                userName = parts[0].Trim();
                                userGender = parts[5].Trim().ToLower();
                                break;
                            }
                        }
                    }

                    if (found)
                    {
                        byte[] ack = Encoding.UTF8.GetBytes("Access granted");
                        await stream.WriteAsync(ack, 0, ack.Length);

                        this.Invoke((MethodInvoker)delegate
                        {
                            Form4 welcome = new Form4(userName, userGender);
                            welcome.Show();
                            this.Hide();
                        });
                    }
                    else
                    {
                        byte[] ack = Encoding.UTF8.GetBytes("Access denied");
                        await stream.WriteAsync(ack, 0, ack.Length);

                        Console.WriteLine("MAC not recognized: " + mac);
                    }

                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Listener error: " + ex.Message);
                    break;
                }
            }

            isListening = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // normal login if needed
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listener != null)
                    listener.Stop();

                isListening = false;
            }
            catch
            {
            }

            Form2 pnn = new Form2();
            this.Hide();
            pnn.Show();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (listener != null)
                    listener.Stop();

                isListening = false;
            }
            catch
            {
            }

            base.OnFormClosing(e);
        }

        private void btnManualLogin_Click(object sender, EventArgs e)
        {
            string enteredUsername = textBox1.Text.Trim();
            string enteredPassword = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(enteredUsername) || string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            bool loginSuccessful = false;
            string foundName = "";
            string foundGender = "male";

            foreach (string line in userLines)
            {
                if (line.StartsWith("name", StringComparison.OrdinalIgnoreCase))
                    continue;

                string[] parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    string username = parts[1].Trim();
                    string password = parts[2].Trim();

                    if (username == enteredUsername && password == enteredPassword)
                    {
                        loginSuccessful = true;
                        foundName = parts[0].Trim();
                        foundGender = parts[5].Trim().ToLower();
                        break;
                    }
                }
            }

            if (loginSuccessful)
            {
                Form4 welcome = new Form4(foundName, foundGender);
                welcome.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Placeholder for old button1 click if still referenced
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 pnn4 = new Form4("alaaa");
            this.Hide();
            pnn4.Show();
        }
    }
}