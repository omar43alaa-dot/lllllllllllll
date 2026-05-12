using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.IO;

namespace WindowsFormsApp21
{
    public partial class Form4 : Form
    {
        private TcpListener server;
        private Thread serverThread;
        private string userGender = "";
        private string username = "";

        // Gaze Tracking Frequency variables
        private Dictionary<string, int> gazeCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "left up", 0 }, { "right up", 0 }, { "left down", 0 }, { "right down", 0 }, { "center", 0 }
        };
        private System.Windows.Forms.Timer adUpdateTimer;
        public Form4(string userName, string gender = "")
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            label1.Text = $"Welcome {userName}";
                username = userName;

            if (string.IsNullOrEmpty(gender))
                gender = GetUserGender(username);
            
            userGender = gender;

            if (userGender.Equals("female", StringComparison.OrdinalIgnoreCase))
            { 
                this.BackColor = Color.Pink;
            }
            else if (gender.Equals("male", StringComparison.OrdinalIgnoreCase))
            {
                this.BackColor = SystemColors.ActiveCaption;
            }

            // Subscribe to emotion updates
            EmotionSocketClient.OnEmotionReceived += UpdateEmotionLabel;
            this.FormClosing += (s, e) => EmotionSocketClient.OnEmotionReceived -= UpdateEmotionLabel;
            
            // Set initial value
            lblEmotion.Text = "Connecting to Camera...";

            // Initialize Advertisement Timer
            adUpdateTimer = new System.Windows.Forms.Timer();
            adUpdateTimer.Interval = 5000; // Check frequency every 5 seconds
            adUpdateTimer.Tick += AdUpdateTimer_Tick;
            adUpdateTimer.Start();
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
            
            // Optional: Change background color based on emotion?
            // if (emotion == "Happy") this.BackColor = Color.LightYellow;
        }
        private string GetUserGender(string username)
        {
            string path = "users.txt";

            if (!File.Exists(path))
                return "";

            foreach (var line in File.ReadAllLines(path))
            {
                if (line.StartsWith("name")) continue;

                string[] parts = line.Split(',');

                if (parts.Length >= 6)
                {
                    string name = parts[0].Trim();
                    string gender = parts[5].Trim();

                    if (name.Equals(username, StringComparison.OrdinalIgnoreCase))
                    {
                        return gender;
                    }
                }
            }

            return "";
        }
        private void StartServer()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, 12345);
                server.Start();

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    this.Invoke((MethodInvoker)delegate
                    {
                        if (message == "LEFT")
                        {
                            button1.PerformClick();
                        }
                        else if (message == "RIGHT")
                        {
                            button2.PerformClick();
                        }
                        
                        // Handle 5-position gaze data
                        string lowerMessage = message.ToLower();
                        if (gazeCounts.ContainsKey(lowerMessage))
                        {
                            gazeCounts[lowerMessage]++;
                        }
                    });

                    client.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server error: " + ex.Message);
            }
        }

        private void AdUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            // Find the position with the maximum count
            string mostFrequent = gazeCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            int maxCount = gazeCounts[mostFrequent];

            if (maxCount > 0)
            {
                UpdateAdPosition(mostFrequent);
                lblAcademyAd.Visible = true;
            }

            // Reset counts for the next interval
            var keys = new List<string>(gazeCounts.Keys);
            foreach (var key in keys) gazeCounts[key] = 0;
        }

        private void UpdateAdPosition(string position)
        {
            int margin = 50;
            int adWidth = lblAcademyAd.Width;
            int adHeight = lblAcademyAd.Height;

            switch (position.ToLower())
            {
                case "left up":
                    lblAcademyAd.Location = new Point(margin, margin);
                    break;
                case "right up":
                    lblAcademyAd.Location = new Point(this.ClientSize.Width - adWidth - margin, margin);
                    break;
                case "left down":
                    lblAcademyAd.Location = new Point(margin, this.ClientSize.Height - adHeight - margin);
                    break;
                case "right down":
                    lblAcademyAd.Location = new Point(this.ClientSize.Width - adWidth - margin, this.ClientSize.Height - adHeight - margin);
                    break;
                case "center":
                    lblAcademyAd.Location = new Point((this.ClientSize.Width - adWidth) / 2, (this.ClientSize.Height - adHeight) / 2);
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serverThread = new Thread(StartServer);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormScores pnn = new FormScores();
            this.Hide(); 
            pnn.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StudentChoiceForm choiceForm = new StudentChoiceForm(username, userGender);
            this.Hide();
            choiceForm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}
