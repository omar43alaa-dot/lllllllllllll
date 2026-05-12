using System.Windows.Forms;

namespace WindowsFormsApp21
{
    partial class StudentChoiceForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnExistingGame = new System.Windows.Forms.Button();
            this.btnCustomGame = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblEmotion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExistingGame
            // 
            this.btnExistingGame.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnExistingGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExistingGame.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.btnExistingGame.ForeColor = System.Drawing.Color.White;
            this.btnExistingGame.Location = new System.Drawing.Point(200, 300);
            this.btnExistingGame.Name = "btnExistingGame";
            this.btnExistingGame.Size = new System.Drawing.Size(500, 300);
            this.btnExistingGame.TabIndex = 0;
            this.btnExistingGame.Text = "Choose the Game Already Made";
            this.btnExistingGame.UseVisualStyleBackColor = false;
            this.btnExistingGame.Click += new System.EventHandler(this.btnExistingGame_Click);
            // 
            // btnCustomGame
            // 
            this.btnCustomGame.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnCustomGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCustomGame.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.btnCustomGame.ForeColor = System.Drawing.Color.White;
            this.btnCustomGame.Location = new System.Drawing.Point(800, 300);
            this.btnCustomGame.Name = "btnCustomGame";
            this.btnCustomGame.Size = new System.Drawing.Size(500, 300);
            this.btnCustomGame.TabIndex = 1;
            this.btnCustomGame.Text = "Implement Your Game";
            this.btnCustomGame.UseVisualStyleBackColor = false;
            this.btnCustomGame.Click += new System.EventHandler(this.btnCustomGame_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(450, 100);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(657, 81);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "What do you want to play?";
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(50, 50);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 40);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblEmotion
            // 
            this.lblEmotion.AutoSize = true;
            this.lblEmotion.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmotion.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblEmotion.Location = new System.Drawing.Point(200, 50);
            this.lblEmotion.Name = "lblEmotion";
            this.lblEmotion.Size = new System.Drawing.Size(124, 32);
            this.lblEmotion.TabIndex = 4;
            this.lblEmotion.Text = "Emotion: -";
            // 
            // StudentChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.lblEmotion);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCustomGame);
            this.Controls.Add(this.btnExistingGame);
            this.Name = "StudentChoiceForm";
            this.Text = "Choose Your Path";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnExistingGame;
        private System.Windows.Forms.Button btnCustomGame;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblEmotion;
    }
}
