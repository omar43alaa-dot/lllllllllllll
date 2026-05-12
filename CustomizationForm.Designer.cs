using System.Windows.Forms;

namespace WindowsFormsApp21
{
    partial class CustomizationForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlAnimals = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlBackgrounds = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlAssets = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSelectedAnimal = new System.Windows.Forms.Label();
            this.lblSelectedBackground = new System.Windows.Forms.Label();
            this.lblSelectedAsset = new System.Windows.Forms.Label();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(450, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(657, 81);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Implement Your Game";
            // 
            // pnlAnimals
            // 
            this.pnlAnimals.AutoScroll = true;
            this.pnlAnimals.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAnimals.Location = new System.Drawing.Point(100, 150);
            this.pnlAnimals.Name = "pnlAnimals";
            this.pnlAnimals.Size = new System.Drawing.Size(1400, 180);
            this.pnlAnimals.TabIndex = 1;
            // 
            // pnlBackgrounds
            // 
            this.pnlBackgrounds.AutoScroll = true;
            this.pnlBackgrounds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBackgrounds.Location = new System.Drawing.Point(100, 380);
            this.pnlBackgrounds.Name = "pnlBackgrounds";
            this.pnlBackgrounds.Size = new System.Drawing.Size(1400, 180);
            this.pnlBackgrounds.TabIndex = 2;
            // 
            // pnlAssets
            // 
            this.pnlAssets.AutoScroll = true;
            this.pnlAssets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAssets.Location = new System.Drawing.Point(100, 610);
            this.pnlAssets.Name = "pnlAssets";
            this.pnlAssets.Size = new System.Drawing.Size(1400, 180);
            this.pnlAssets.TabIndex = 3;
            // 
            // lblSelectedAnimal
            // 
            this.lblSelectedAnimal.AutoSize = true;
            this.lblSelectedAnimal.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblSelectedAnimal.Location = new System.Drawing.Point(100, 115);
            this.lblSelectedAnimal.Name = "lblSelectedAnimal";
            this.lblSelectedAnimal.Size = new System.Drawing.Size(262, 32);
            this.lblSelectedAnimal.TabIndex = 4;
            this.lblSelectedAnimal.Text = "Select Favorite Animal:";
            // 
            // lblSelectedBackground
            // 
            this.lblSelectedBackground.AutoSize = true;
            this.lblSelectedBackground.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblSelectedBackground.Location = new System.Drawing.Point(100, 345);
            this.lblSelectedBackground.Name = "lblSelectedBackground";
            this.lblSelectedBackground.Size = new System.Drawing.Size(215, 32);
            this.lblSelectedBackground.TabIndex = 5;
            this.lblSelectedBackground.Text = "Select Background:";
            // 
            // lblSelectedAsset
            // 
            this.lblSelectedAsset.AutoSize = true;
            this.lblSelectedAsset.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblSelectedAsset.Location = new System.Drawing.Point(100, 575);
            this.lblSelectedAsset.Name = "lblSelectedAsset";
            this.lblSelectedAsset.Size = new System.Drawing.Size(146, 32);
            this.lblSelectedAsset.TabIndex = 6;
            this.lblSelectedAsset.Text = "Select Asset:";
            // 
            // btnStartGame
            // 
            this.btnStartGame.BackColor = System.Drawing.Color.LimeGreen;
            this.btnStartGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartGame.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.btnStartGame.ForeColor = System.Drawing.Color.White;
            this.btnStartGame.Location = new System.Drawing.Point(600, 850);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(400, 100);
            this.btnStartGame.TabIndex = 7;
            this.btnStartGame.Text = "START GAME";
            this.btnStartGame.UseVisualStyleBackColor = false;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(50, 30);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 40);
            this.btnBack.TabIndex = 8;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // CustomizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnStartGame);
            this.Controls.Add(this.lblSelectedAsset);
            this.Controls.Add(this.lblSelectedBackground);
            this.Controls.Add(this.lblSelectedAnimal);
            this.Controls.Add(this.pnlAssets);
            this.Controls.Add(this.pnlBackgrounds);
            this.Controls.Add(this.pnlAnimals);
            this.Controls.Add(this.lblTitle);
            this.Name = "CustomizationForm";
            this.Text = "Customize Your Game";
            this.Load += new System.EventHandler(this.CustomizationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel pnlAnimals;
        private System.Windows.Forms.FlowLayoutPanel pnlBackgrounds;
        private System.Windows.Forms.FlowLayoutPanel pnlAssets;
        private System.Windows.Forms.Label lblSelectedAnimal;
        private System.Windows.Forms.Label lblSelectedBackground;
        private System.Windows.Forms.Label lblSelectedAsset;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.Button btnBack;
    }
}
