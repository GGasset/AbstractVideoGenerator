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
            this.Display = new System.Windows.Forms.PictureBox();
            this.ShowAutoencoderImageBttn = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AutoencoderVideoSelectedImageBttn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Display)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Display
            // 
            this.Display.BackColor = System.Drawing.Color.DarkOrange;
            this.Display.Location = new System.Drawing.Point(3, 12);
            this.Display.Name = "Display";
            this.Display.Size = new System.Drawing.Size(150, 150);
            this.Display.TabIndex = 2;
            this.Display.TabStop = false;
            // 
            // ShowAutoencoderImageBttn
            // 
            this.ShowAutoencoderImageBttn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowAutoencoderImageBttn.Location = new System.Drawing.Point(6, 19);
            this.ShowAutoencoderImageBttn.Name = "ShowAutoencoderImageBttn";
            this.ShowAutoencoderImageBttn.Size = new System.Drawing.Size(415, 23);
            this.ShowAutoencoderImageBttn.TabIndex = 3;
            this.ShowAutoencoderImageBttn.Text = "Show autoencoder image";
            this.ShowAutoencoderImageBttn.UseVisualStyleBackColor = true;
            this.ShowAutoencoderImageBttn.Click += new System.EventHandler(this.ShowAutoencoderImageBttn_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(6, 77);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(415, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Show autoencoder video from random image";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(159, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(437, 150);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox1.Controls.Add(this.AutoencoderVideoSelectedImageBttn);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.ShowAutoencoderImageBttn);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 110);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Autoencoder network";
            // 
            // AutoencoderVideoSelectedImageBttn
            // 
            this.AutoencoderVideoSelectedImageBttn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoencoderVideoSelectedImageBttn.Location = new System.Drawing.Point(6, 48);
            this.AutoencoderVideoSelectedImageBttn.Name = "AutoencoderVideoSelectedImageBttn";
            this.AutoencoderVideoSelectedImageBttn.Size = new System.Drawing.Size(415, 23);
            this.AutoencoderVideoSelectedImageBttn.TabIndex = 6;
            this.AutoencoderVideoSelectedImageBttn.Text = "Show autoencoder video from image";
            this.AutoencoderVideoSelectedImageBttn.UseVisualStyleBackColor = true;
            this.AutoencoderVideoSelectedImageBttn.Click += new System.EventHandler(this.AutoencoderVideoSelectedImageBttn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(598, 174);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.Display);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Video Generator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Display)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromFileToolStripMenuItem;
        private System.Windows.Forms.PictureBox Display;
        private System.Windows.Forms.Button ShowAutoencoderImageBttn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button AutoencoderVideoSelectedImageBttn;
    }
}

