namespace DiscordBot
{
    partial class BtnSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BtnSettings));
            this.closeBtn = new System.Windows.Forms.Button();
            this.btnTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.filechooser = new System.Windows.Forms.OpenFileDialog();
            this.fileBtn = new System.Windows.Forms.Button();
            this.fileChooseBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.Location = new System.Drawing.Point(357, 234);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 4;
            this.closeBtn.Text = "Exit";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnTextBox
            // 
            this.btnTextBox.Location = new System.Drawing.Point(15, 25);
            this.btnTextBox.Name = "btnTextBox";
            this.btnTextBox.Size = new System.Drawing.Size(225, 20);
            this.btnTextBox.TabIndex = 1;
            this.btnTextBox.TextChanged += new System.EventHandler(this.btnTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Button name:";
            // 
            // filechooser
            // 
            this.filechooser.FileName = "fileChooser";
            // 
            // fileBtn
            // 
            this.fileBtn.Location = new System.Drawing.Point(15, 119);
            this.fileBtn.Name = "fileBtn";
            this.fileBtn.Size = new System.Drawing.Size(75, 23);
            this.fileBtn.TabIndex = 3;
            this.fileBtn.Text = "Choose...";
            this.fileBtn.UseVisualStyleBackColor = true;
            this.fileBtn.Click += new System.EventHandler(this.fileBtn_Click);
            // 
            // fileChooseBox
            // 
            this.fileChooseBox.Location = new System.Drawing.Point(15, 93);
            this.fileChooseBox.Name = "fileChooseBox";
            this.fileChooseBox.Size = new System.Drawing.Size(225, 20);
            this.fileChooseBox.TabIndex = 2;
            this.fileChooseBox.TextChanged += new System.EventHandler(this.fileChooseBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Audio file:";
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::DiscordBot.Properties.Resources.paypal;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Location = new System.Drawing.Point(12, 210);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 47);
            this.button1.TabIndex = 5;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // BtnSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 269);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fileChooseBox);
            this.Controls.Add(this.fileBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTextBox);
            this.Controls.Add(this.closeBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BtnSettings";
            this.Text = "BtnSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.TextBox btnTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog filechooser;
        private System.Windows.Forms.Button fileBtn;
        private System.Windows.Forms.TextBox fileChooseBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}