using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp21
{
    public partial class FormScores : Form
    {
        public FormScores()
        {
            InitializeComponent();
            LoadScores();
            this.WindowState = FormWindowState.Maximized;

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

        private void LoadScores()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "Student Name";
            dataGridView1.Columns[1].Name = "Score";

            string path = Path.Combine(Application.StartupPath, "users.txt");

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (line.StartsWith("name", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length >= 8)
                    {
                        string studentName = parts[0].Trim();
                        string score = parts[7].Trim();

                        dataGridView1.Rows.Add(studentName, score);
                    }
                }
            }
            else
            {
                MessageBox.Show("users.txt not found:\n" + path);
            }

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string studentToDelete = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                string path = Path.Combine(Application.StartupPath, "users.txt");

                if (File.Exists(path))
                {
                    var lines = File.ReadAllLines(path);
                    var newLines = new System.Collections.Generic.List<string>();

                    bool deleted = false;
                    foreach (var line in lines)
                    {
                        if (line.StartsWith(studentToDelete + ","))
                        {
                            deleted = true;
                            continue;
                        }
                        newLines.Add(line);
                    }

                    if (deleted)
                    {
                        File.WriteAllLines(path, newLines);
                        MessageBox.Show("Student " + studentToDelete + " deleted successfully.");
                        LoadScores();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)Application.OpenForms["Form1"];
            if (f1 != null)
            {
                f1.Show();
                this.Hide();
            }
            else
            {
                f1 = new Form1();
                this.Hide();
                f1.Show();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}