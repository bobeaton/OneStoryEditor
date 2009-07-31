namespace OneStoryProjectEditor
{
	partial class NetBibleFootnoteTooltip
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
			this.lblText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblText
			// 
			this.lblText.AutoSize = true;
			this.lblText.BackColor = System.Drawing.Color.Transparent;
			this.lblText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblText.Location = new System.Drawing.Point(1, 0);
            this.lblText.MaximumSize = new System.Drawing.Size(400, 800);
            this.lblText.MinimumSize = new System.Drawing.Size(50, 15);
			this.lblText.Name = "lblText";
			this.lblText.Padding = new System.Windows.Forms.Padding(5);
            this.lblText.Size = new System.Drawing.Size(50, 25);
			this.lblText.TabIndex = 2;
			// 
			// NetBibleFootnoteTooltip
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(368, 202);
			this.ControlBox = false;
			this.Controls.Add(this.lblText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 800);
			this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(25, 0);
			this.Name = "NetBibleFootnoteTooltip";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblText;


	}
}