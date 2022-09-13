namespace AbstractVideoGenerator
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainFromImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.Display = new System.Windows.Forms.PictureBox();
            this.ShowAutoencoderImageBttn = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Display)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.trainToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(515, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "Menu";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToFolderToolStripMenuItem});
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveToFolderToolStripMenuItem
            // 
            this.saveToFolderToolStripMenuItem.Name = "saveToFolderToolStripMenuItem";
            this.saveToFolderToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToFolderToolStripMenuItem.Text = "Save to folder";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFromFileToolStripMenuItem});
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // loadFromFileToolStripMenuItem
            // 
            this.loadFromFileToolStripMenuItem.Name = "loadFromFileToolStripMenuItem";
            this.loadFromFileToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.loadFromFileToolStripMenuItem.Text = "Load from file";
            // 
            // trainToolStripMenuItem
            // 
            this.trainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trainFromImagesToolStripMenuItem});
            this.trainToolStripMenuItem.Name = "trainToolStripMenuItem";
            this.trainToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.trainToolStripMenuItem.Text = "Train";
            // 
            // trainFromImagesToolStripMenuItem
            // 
            this.trainFromImagesToolStripMenuItem.Name = "trainFromImagesToolStripMenuItem";
            this.trainFromImagesToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.trainFromImagesToolStripMenuItem.Text = "Train from images";
            this.trainFromImagesToolStripMenuItem.Click += new System.EventHandler(this.trainFromImagesToolStripMenuItem_Click);
            // 
            // comboBox
            // 
            this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(5, 27);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(505, 21);
            this.comboBox.TabIndex = 1;
            // 
            // Display
            // 
            this.Display.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Display.Location = new System.Drawing.Point(0, 54);
            this.Display.Name = "Display";
            this.Display.Size = new System.Drawing.Size(150, 150);
            this.Display.TabIndex = 2;
            this.Display.TabStop = false;
            // 
            // ShowAutoencoderImageBttn
            // 
            this.ShowAutoencoderImageBttn.Location = new System.Drawing.Point(156, 54);
            this.ShowAutoencoderImageBttn.Name = "ShowAutoencoderImageBttn";
            this.ShowAutoencoderImageBttn.Size = new System.Drawing.Size(143, 23);
            this.ShowAutoencoderImageBttn.TabIndex = 3;
            this.ShowAutoencoderImageBttn.Text = "Show autoencoder image";
            this.ShowAutoencoderImageBttn.UseVisualStyleBackColor = true;
            this.ShowAutoencoderImageBttn.Click += new System.EventHandler(this.ShowAutoencoderImageBttn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(515, 209);
            this.Controls.Add(this.ShowAutoencoderImageBttn);
            this.Controls.Add(this.Display);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Video Generator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Display)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trainFromImagesToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.PictureBox Display;
        private System.Windows.Forms.Button ShowAutoencoderImageBttn;
    }
}

