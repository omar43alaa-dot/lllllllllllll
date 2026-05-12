using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp21
{
    public partial class Form2 : Form
    {
        String name = "";

        String username = "";
        String email = "";
        String password = "";
        String photo = "";
        String bluetoothid = "";
        string gender = "";
        string Score = "0";


        public Form2()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
           
            ofd.Title = "Choose a photo";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(ofd.FileName);

                photo = ofd.FileName;
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form1 pnn=new Form1();
            this.Hide();
            pnn.ShowDialog();
                 }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "\\users.txt";

            using (StreamWriter sw = new StreamWriter(path, true)) 
            {
                username = textBox4.Text;
                password = textBox1.Text;
                email = textBox5.Text;
                bluetoothid = textBox3.Text;
                if(radioButton2.Checked)
                {
                    gender = "male";
                }
                else
                {
                    gender = "female";
                }
                name = textBox2.Text;
                if (username != "" && password != "" && email != "" && bluetoothid != "" && gender != "")
                {

                    sw.WriteLine($"{name},{username},{password},{email},{bluetoothid},{gender},{photo},{Score}");
                }
                else
                {
                    MessageBox.Show("please enter your information full");
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
