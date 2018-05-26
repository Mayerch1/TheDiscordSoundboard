namespace DiscordBot
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.connectBtn = new System.Windows.Forms.Button();
            this.abortBtn = new System.Windows.Forms.Button();
            this.channelIdBox = new System.Windows.Forms.TextBox();
            this.channelIdLbl = new System.Windows.Forms.Label();
            this.tokenButton = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.filePage = new System.Windows.Forms.TabPage();
            this.flowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNumBox = new System.Windows.Forms.NumericUpDown();
            this.volumeSlider = new System.Windows.Forms.TrackBar();
            this.earRapeBox = new System.Windows.Forms.CheckBox();
            this.loopBox = new System.Windows.Forms.CheckBox();
            this.connectContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectContextEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl.SuspendLayout();
            this.filePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeSlider)).BeginInit();
            this.connectContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectBtn
            // 
            this.connectBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connectBtn.Location = new System.Drawing.Point(4, 273);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 1;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            this.connectBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.connectBtn_MouseDown);
            // 
            // abortBtn
            // 
            this.abortBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.abortBtn.Location = new System.Drawing.Point(762, 273);
            this.abortBtn.Name = "abortBtn";
            this.abortBtn.Size = new System.Drawing.Size(75, 23);
            this.abortBtn.TabIndex = 8;
            this.abortBtn.Text = "Abort";
            this.abortBtn.UseVisualStyleBackColor = true;
            this.abortBtn.Click += new System.EventHandler(this.abortBtn_Click);
            // 
            // channelIdBox
            // 
            this.channelIdBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.channelIdBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.channelIdBox.Location = new System.Drawing.Point(154, 275);
            this.channelIdBox.Name = "channelIdBox";
            this.channelIdBox.Size = new System.Drawing.Size(124, 20);
            this.channelIdBox.TabIndex = 2;
            this.channelIdBox.TextChanged += new System.EventHandler(this.channelIdBox_TextChanged);
            // 
            // channelIdLbl
            // 
            this.channelIdLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.channelIdLbl.AutoSize = true;
            this.channelIdLbl.Location = new System.Drawing.Point(85, 278);
            this.channelIdLbl.Name = "channelIdLbl";
            this.channelIdLbl.Size = new System.Drawing.Size(63, 13);
            this.channelIdLbl.TabIndex = 9;
            this.channelIdLbl.Text = "Channel ID:";
            // 
            // tokenButton
            // 
            this.tokenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tokenButton.Location = new System.Drawing.Point(681, 273);
            this.tokenButton.Name = "tokenButton";
            this.tokenButton.Size = new System.Drawing.Size(75, 23);
            this.tokenButton.TabIndex = 7;
            this.tokenButton.Text = "Token...";
            this.tokenButton.UseVisualStyleBackColor = true;
            this.tokenButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.filePage);
            this.tabControl.Location = new System.Drawing.Point(4, 2);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(833, 265);
            this.tabControl.TabIndex = 10;
            // 
            // filePage
            // 
            this.filePage.Controls.Add(this.flowLayout);
            this.filePage.Location = new System.Drawing.Point(4, 22);
            this.filePage.Name = "filePage";
            this.filePage.Padding = new System.Windows.Forms.Padding(3);
            this.filePage.Size = new System.Drawing.Size(825, 239);
            this.filePage.TabIndex = 0;
            this.filePage.Text = "Files";
            this.filePage.UseVisualStyleBackColor = true;
            // 
            // flowLayout
            // 
            this.flowLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayout.Location = new System.Drawing.Point(6, 6);
            this.flowLayout.Name = "flowLayout";
            this.flowLayout.Size = new System.Drawing.Size(813, 230);
            this.flowLayout.TabIndex = 4;
            // 
            // btnNumBox
            // 
            this.btnNumBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNumBox.Location = new System.Drawing.Point(620, 275);
            this.btnNumBox.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.btnNumBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.btnNumBox.Name = "btnNumBox";
            this.btnNumBox.Size = new System.Drawing.Size(55, 20);
            this.btnNumBox.TabIndex = 6;
            this.btnNumBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.btnNumBox.ValueChanged += new System.EventHandler(this.btnNumBox_ValueChanged);
            // 
            // volumeSlider
            // 
            this.volumeSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volumeSlider.LargeChange = 10;
            this.volumeSlider.Location = new System.Drawing.Point(284, 275);
            this.volumeSlider.Maximum = 200;
            this.volumeSlider.Name = "volumeSlider";
            this.volumeSlider.Size = new System.Drawing.Size(171, 45);
            this.volumeSlider.TabIndex = 3;
            this.volumeSlider.TickFrequency = 25;
            this.volumeSlider.Value = 25;
            // 
            // earRapeBox
            // 
            this.earRapeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.earRapeBox.AutoSize = true;
            this.earRapeBox.Location = new System.Drawing.Point(461, 277);
            this.earRapeBox.Name = "earRapeBox";
            this.earRapeBox.Size = new System.Drawing.Size(63, 17);
            this.earRapeBox.TabIndex = 4;
            this.earRapeBox.Text = "Earrape";
            this.earRapeBox.UseVisualStyleBackColor = true;
            // 
            // loopBox
            // 
            this.loopBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loopBox.AutoSize = true;
            this.loopBox.Location = new System.Drawing.Point(530, 277);
            this.loopBox.Name = "loopBox";
            this.loopBox.Size = new System.Drawing.Size(50, 17);
            this.loopBox.TabIndex = 5;
            this.loopBox.Text = "Loop";
            this.loopBox.UseVisualStyleBackColor = true;
            this.loopBox.CheckedChanged += new System.EventHandler(this.loopBox_CheckedChanged);
            // 
            // connectContext
            // 
            this.connectContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectContextEntry});
            this.connectContext.Name = "connectContext";
            this.connectContext.Size = new System.Drawing.Size(144, 26);
            // 
            // connectContextEntry
            // 
            this.connectContextEntry.Name = "connectContextEntry";
            this.connectContextEntry.Size = new System.Drawing.Size(143, 22);
            this.connectContextEntry.Text = "Autoconnect";
            this.connectContextEntry.Click += new System.EventHandler(this.connectContextEntry_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 308);
            this.Controls.Add(this.loopBox);
            this.Controls.Add(this.btnNumBox);
            this.Controls.Add(this.earRapeBox);
            this.Controls.Add(this.tokenButton);
            this.Controls.Add(this.volumeSlider);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.channelIdLbl);
            this.Controls.Add(this.channelIdBox);
            this.Controls.Add(this.abortBtn);
            this.Controls.Add(this.connectBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "SoundBoard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.tabControl.ResumeLayout(false);
            this.filePage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeSlider)).EndInit();
            this.connectContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Button abortBtn;
        private System.Windows.Forms.TextBox channelIdBox;
        private System.Windows.Forms.Label channelIdLbl;
        private System.Windows.Forms.Button tokenButton;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage filePage;
        private System.Windows.Forms.FlowLayoutPanel flowLayout;
        private System.Windows.Forms.TrackBar volumeSlider;
        private System.Windows.Forms.CheckBox earRapeBox;
        private System.Windows.Forms.CheckBox loopBox;
        private System.Windows.Forms.NumericUpDown btnNumBox;
        private System.Windows.Forms.ContextMenuStrip connectContext;
        private System.Windows.Forms.ToolStripMenuItem connectContextEntry;
    }
}

