namespace NVSXBOX
{
    partial class Form_SpeedSetting
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
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.Speed = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Angular = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.Speed.SuspendLayout();
            this.Angular.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(3, 21);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(438, 45);
            this.trackBar1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.trackBar1, "123456");
            this.trackBar1.Value = 8;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // Speed
            // 
            this.Speed.Controls.Add(this.label1);
            this.Speed.Controls.Add(this.trackBar1);
            this.Speed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Speed.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Speed.Location = new System.Drawing.Point(8, 9);
            this.Speed.Name = "Speed";
            this.Speed.Size = new System.Drawing.Size(564, 69);
            this.Speed.TabIndex = 2;
            this.Speed.TabStop = false;
            this.Speed.Text = " Linear Speed";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(463, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "4 m/s";
            // 
            // Angular
            // 
            this.Angular.Controls.Add(this.label2);
            this.Angular.Controls.Add(this.trackBar2);
            this.Angular.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Angular.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Angular.Location = new System.Drawing.Point(7, 79);
            this.Angular.Name = "Angular";
            this.Angular.Size = new System.Drawing.Size(564, 69);
            this.Angular.TabIndex = 3;
            this.Angular.TabStop = false;
            this.Angular.Text = "Angular Speed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(461, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "45 deg/s";
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(3, 21);
            this.trackBar2.Maximum = 100;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(440, 45);
            this.trackBar2.TabIndex = 0;
            this.trackBar2.Value = 45;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar2_ValueChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // Form_SpeedSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 156);
            this.Controls.Add(this.Angular);
            this.Controls.Add(this.Speed);
            this.Name = "Form_SpeedSetting";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Speed Setting";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.Speed.ResumeLayout(false);
            this.Speed.PerformLayout();
            this.Angular.ResumeLayout(false);
            this.Angular.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.GroupBox Speed;
        private System.Windows.Forms.GroupBox Angular;
        public System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip2;
    }
}