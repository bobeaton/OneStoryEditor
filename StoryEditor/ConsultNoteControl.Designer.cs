namespace OneStoryProjectEditor
{
    partial class ConsultNoteControl
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
            this.labelRound = new System.Windows.Forms.Label();
            this.buttonDragDropHandle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelRound
            // 
            this.labelRound.AutoSize = true;
            this.labelRound.Location = new System.Drawing.Point(3, 0);
            this.labelRound.Name = "labelRound";
            this.labelRound.Size = new System.Drawing.Size(79, 13);
            this.labelRound.TabIndex = 0;
            this.labelRound.Text = "labelRound";
            // 
            // buttonDragDropHandle
            // 
            this.buttonDragDropHandle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonDragDropHandle.Location = new System.Drawing.Point(0, 0);
            this.buttonDragDropHandle.Margin = new System.Windows.Forms.Padding(0);
            this.buttonDragDropHandle.MaximumSize = new System.Drawing.Size(15, 15);
            this.buttonDragDropHandle.Name = "buttonDragDropHandle";
            this.buttonDragDropHandle.Size = new System.Drawing.Size(15, 15);
            this.buttonDragDropHandle.TabIndex = 1;
            this.buttonDragDropHandle.UseVisualStyleBackColor = true;
            // 
            // ConsultNoteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3);
            this.Name = "ConsultNoteControl";
            this.Size = new System.Drawing.Size(669, 225);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelRound;
        private System.Windows.Forms.Button buttonDragDropHandle;
    }
}
