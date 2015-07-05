namespace AutoPCSX2SaveState
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
            this.btnStartStop = new System.Windows.Forms.Button();
            this.nudSaveInterval = new System.Windows.Forms.NumericUpDown();
            this.nudButtonDelay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblHead = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTimeUntilSave = new System.Windows.Forms.Label();
            this.lblNumSaves = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaveInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudButtonDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnStartStop.Location = new System.Drawing.Point(0, 132);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(271, 23);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // nudSaveInterval
            // 
            this.nudSaveInterval.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSaveInterval.Location = new System.Drawing.Point(159, 21);
            this.nudSaveInterval.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudSaveInterval.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudSaveInterval.Name = "nudSaveInterval";
            this.nudSaveInterval.Size = new System.Drawing.Size(91, 20);
            this.nudSaveInterval.TabIndex = 1;
            this.nudSaveInterval.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // nudButtonDelay
            // 
            this.nudButtonDelay.Location = new System.Drawing.Point(159, 47);
            this.nudButtonDelay.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudButtonDelay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudButtonDelay.Name = "nudButtonDelay";
            this.nudButtonDelay.Size = new System.Drawing.Size(91, 20);
            this.nudButtonDelay.TabIndex = 2;
            this.nudButtonDelay.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Time between saves";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Delay after button press";
            // 
            // lblHead
            // 
            this.lblHead.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHead.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHead.Location = new System.Drawing.Point(0, 0);
            this.lblHead.Name = "lblHead";
            this.lblHead.Size = new System.Drawing.Size(271, 15);
            this.lblHead.TabIndex = 5;
            this.lblHead.Text = "All times are expressed in seconds";
            this.lblHead.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Time until next save";
            // 
            // lblTimeUntilSave
            // 
            this.lblTimeUntilSave.AutoSize = true;
            this.lblTimeUntilSave.Location = new System.Drawing.Point(156, 76);
            this.lblTimeUntilSave.Name = "lblTimeUntilSave";
            this.lblTimeUntilSave.Size = new System.Drawing.Size(57, 13);
            this.lblTimeUntilSave.TabIndex = 7;
            this.lblTimeUntilSave.Text = "X seconds";
            // 
            // lblNumSaves
            // 
            this.lblNumSaves.AutoSize = true;
            this.lblNumSaves.Location = new System.Drawing.Point(156, 104);
            this.lblNumSaves.Name = "lblNumSaves";
            this.lblNumSaves.Size = new System.Drawing.Size(13, 13);
            this.lblNumSaves.TabIndex = 8;
            this.lblNumSaves.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Number of times saved";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 155);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblNumSaves);
            this.Controls.Add(this.lblTimeUntilSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblHead);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudButtonDelay);
            this.Controls.Add(this.nudSaveInterval);
            this.Controls.Add(this.btnStartStop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Automatic state saver";
            ((System.ComponentModel.ISupportInitialize)(this.nudSaveInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudButtonDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.NumericUpDown nudSaveInterval;
        private System.Windows.Forms.NumericUpDown nudButtonDelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblHead;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTimeUntilSave;
        private System.Windows.Forms.Label lblNumSaves;
        private System.Windows.Forms.Label label6;
    }
}

