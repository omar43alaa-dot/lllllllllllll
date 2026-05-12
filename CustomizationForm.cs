using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp21
{
    public partial class CustomizationForm : Form
    {
        private string username;
        private string selectedAnimal = "Lion";
        private string selectedBackground = "Zoo.jpg";
        private string selectedAsset = "cage.png";
        private string userGender;

        public CustomizationForm(string username, string gender = "male")
        {
            InitializeComponent();
            this.username = username;
            this.userGender = gender;
            this.WindowState = FormWindowState.Maximized;

            if (userGender.Equals("female", StringComparison.OrdinalIgnoreCase))
                this.BackColor = Color.LavenderBlush;
            else
                this.BackColor = Color.LightSteelBlue;
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            // Open TuioDemo2 with selected settings and gender
            TuioDemo2 game = new TuioDemo2(3333, username, selectedAnimal, selectedBackground, selectedAsset, userGender);
            this.Hide();
            game.Show();
        }

        private void AnimalSelection_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            selectedAnimal = pb.Tag.ToString();
            lblSelectedAnimal.Text = "Selected Animal Image: " + selectedAnimal;
            
            // Highlight selection
            foreach (Control c in pnlAnimals.Controls)
            {
                if (c is PictureBox) {
                    c.Padding = new Padding(0);
                    c.BackColor = Color.Transparent;
                }
            }
            pb.Padding = new Padding(5);
            pb.BackColor = Color.Yellow;
        }

        private void BackgroundSelection_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            selectedBackground = pb.Tag.ToString();
            lblSelectedBackground.Text = "Selected Background: " + selectedBackground;

            foreach (Control c in pnlBackgrounds.Controls)
            {
                if (c is PictureBox) {
                    c.Padding = new Padding(0);
                    c.BackColor = Color.Transparent;
                }
            }
            pb.Padding = new Padding(5);
            pb.BackColor = Color.Yellow;
        }

        private void AssetSelection_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            selectedAsset = pb.Tag.ToString();
            lblSelectedAsset.Text = "Selected Asset: " + selectedAsset;

            foreach (Control c in pnlAssets.Controls)
            {
                if (c is PictureBox) {
                    c.Padding = new Padding(0);
                    c.BackColor = Color.Transparent;
                }
            }
            pb.Padding = new Padding(5);
            pb.BackColor = Color.Yellow;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            StudentChoiceForm choice = new StudentChoiceForm(username, userGender);
            this.Hide();
            choice.Show();
        }

        private void CustomizationForm_Load(object sender, EventArgs e)
        {
            // Populate Animals
            string[] animals = { "Lion", "Crocodile", "Monkey", "Bird" };
            string[] animalImg = { "b1.jpeg", "a3.jpeg", "m1.jpeg", "4.png" };
            for (int i = 0; i < animals.Length; i++)
            {
                PictureBox pb = new PictureBox();
                pb.Size = new System.Drawing.Size(150, 150);
                pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                pb.Tag = animalImg[i]; // Store filename as tag
                if (System.IO.File.Exists(animalImg[i])) pb.Image = System.Drawing.Image.FromFile(animalImg[i]);
                pb.Click += new System.EventHandler(this.AnimalSelection_Click);
                pnlAnimals.Controls.Add(pb);
            }

            // Populate Backgrounds
            string[] bgs = { "zoo.jpg", "h1.jpeg", "h2.jpeg", "h3.jpeg", "h4.jpeg" };
            foreach (string bg in bgs)
            {
                PictureBox pb = new PictureBox();
                pb.Size = new System.Drawing.Size(150, 150);
                pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                pb.Tag = bg;
                if (System.IO.File.Exists(bg)) pb.Image = System.Drawing.Image.FromFile(bg);
                pb.Click += new System.EventHandler(this.BackgroundSelection_Click);
                pnlBackgrounds.Controls.Add(pb);
            }

            // Populate Assets
            string[] assets = { "cage.png", "f1.jpeg", "f2.jpeg", "f3.jpeg" };
            foreach (string asset in assets)
            {
                PictureBox pb = new PictureBox();
                pb.Size = new System.Drawing.Size(150, 150);
                pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                pb.Tag = asset;
                if (System.IO.File.Exists(asset)) pb.Image = System.Drawing.Image.FromFile(asset);
                pb.Click += new System.EventHandler(this.AssetSelection_Click);
                pnlAssets.Controls.Add(pb);
            }
        }
    }
}
