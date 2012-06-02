namespace OneStoryProjectEditor
{
    partial class StateTransitionHistoryGraphForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StateTransitionHistoryGraphForm));
            this.chartTurnTiming = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartTurnTiming)).BeginInit();
            this.SuspendLayout();
            // 
            // chartTurnTiming
            // 
            chartArea1.Name = "ChartArea1";
            this.chartTurnTiming.ChartAreas.Add(chartArea1);
            this.chartTurnTiming.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartTurnTiming.Legends.Add(legend1);
            this.chartTurnTiming.Location = new System.Drawing.Point(0, 0);
            this.chartTurnTiming.Name = "chartTurnTiming";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Days in turn";
            this.chartTurnTiming.Series.Add(series1);
            this.chartTurnTiming.Size = new System.Drawing.Size(642, 351);
            this.chartTurnTiming.TabIndex = 2;
            this.chartTurnTiming.Text = "Time in turn chart";
            // 
            // StateTransitionHistoryGraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 351);
            this.Controls.Add(this.chartTurnTiming);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StateTransitionHistoryGraphForm";
            this.Text = "Time in Turn";
            ((System.ComponentModel.ISupportInitialize)(this.chartTurnTiming)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartTurnTiming;
    }
}