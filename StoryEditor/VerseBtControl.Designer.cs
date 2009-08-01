namespace OneStoryProjectEditor
{
    partial class VerseBtControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelReference = new System.Windows.Forms.Label();
            this.buttonDragDropHandle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelReference
            // 
            this.labelReference.AutoSize = true;
            this.labelReference.Location = new System.Drawing.Point(3, 0);
            this.labelReference.Name = "labelReference";
            this.labelReference.Size = new System.Drawing.Size(79, 13);
            this.labelReference.TabIndex = 0;
            this.labelReference.Text = "labelReference";
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
            this.buttonDragDropHandle.MouseDown += new System.Windows.Forms.MouseEventHandler(buttonDragDropHandle_MouseDown);
            this.buttonDragDropHandle.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(buttonDragDropHandle_QueryContinueDrag);
            // 
            // VerseBtControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3);
            this.Name = "VerseBtControl";
            this.Size = new System.Drawing.Size(669, 225);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelReference;
        private System.Windows.Forms.Button buttonDragDropHandle;
    }
}
