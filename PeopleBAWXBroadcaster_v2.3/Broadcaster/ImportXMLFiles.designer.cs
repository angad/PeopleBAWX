namespace Broadcaster
{
    partial class Import
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
            this.source_path = new System.Windows.Forms.TextBox();
            this.source = new System.Windows.Forms.Button();
            this.imp = new System.Windows.Forms.Button();
            this.file_Copy = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // source_path
            // 
            this.source_path.Location = new System.Drawing.Point(40, 44);
            this.source_path.Name = "source_path";
            this.source_path.Size = new System.Drawing.Size(145, 20);
            this.source_path.TabIndex = 2;
            // 
            // source
            // 
            this.source.Location = new System.Drawing.Point(191, 39);
            this.source.Name = "source";
            this.source.Size = new System.Drawing.Size(70, 29);
            this.source.TabIndex = 3;
            this.source.Text = "Source";
            this.source.UseVisualStyleBackColor = true;
            this.source.Click += new System.EventHandler(this.source_Click);
            // 
            // imp
            // 
            this.imp.Location = new System.Drawing.Point(96, 151);
            this.imp.Name = "imp";
            this.imp.Size = new System.Drawing.Size(102, 36);
            this.imp.TabIndex = 4;
            this.imp.Text = "Import";
            this.imp.UseVisualStyleBackColor = true;
            this.imp.Click += new System.EventHandler(this.imp_Click);
            // 
            // file_Copy
            // 
            this.file_Copy.Location = new System.Drawing.Point(40, 217);
            this.file_Copy.Name = "file_Copy";
            this.file_Copy.Size = new System.Drawing.Size(221, 22);
            this.file_Copy.TabIndex = 5;
            // 
            // import
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.file_Copy);
            this.Controls.Add(this.imp);
            this.Controls.Add(this.source);
            this.Controls.Add(this.source_path);
            this.Name = "import";
            this.Text = "import";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox source_path;
        private System.Windows.Forms.Button source;
        private System.Windows.Forms.Button imp;
        private System.Windows.Forms.ProgressBar file_Copy;
    }
}