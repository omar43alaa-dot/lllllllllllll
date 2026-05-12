using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp21
{
    public partial class StudentChoiceForm : Form
    {
        private string username;
        private string userGender;

        public StudentChoiceForm(string username, string gender = "male")
        {
            InitializeComponent();
            this.username = username;
            this.userGender = gender;
            this.WindowState = FormWindowState.Maximized;
            
            if (userGender.Equals("female", StringComparison.OrdinalIgnoreCase))
                this.BackColor = Color.MistyRose;
            else
                this.BackColor = Color.AliceBlue;

            // Subscribe to emotion updates
            EmotionSocketClient.OnEmotionReceived += UpdateEmotionLabel;
            this.FormClosing += (s, e) => EmotionSocketClient.OnEmotionReceived -= UpdateEmotionLabel;
            
            // Set initial value
            lblEmotion.Text = $"Emotion: {EmotionSocketClient.CurrentEmotion}";
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

        private void btnExistingGame_Click(object sender, EventArgs e)
        {
            // Open the normal game with default settings and gender
            TuioDemo2 game = new TuioDemo2(3333, username, "default", "Zoo.jpg", "default", userGender);
            this.Hide();
            game.Show();
        }

        private void btnCustomGame_Click(object sender, EventArgs e)
        {
            // Open customization form with gender
            CustomizationForm customForm = new CustomizationForm(username, userGender);
            this.Hide();
            customForm.Show();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4(username, userGender);
            this.Hide();
            form4.Show();
        }
    }
}
