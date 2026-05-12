using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp21
{
    public partial class Form3 : Form
    {
        public Form3(string userName)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            label2.Text = $"Welcome {userName}";
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
           
        }
    }
}
