
namespace Demo14_WinFormCarDetectionCascadesVsDnn
{
    partial class FormCarDetectionComparison
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.pictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.pictureBoxCascade = new System.Windows.Forms.PictureBox();
            this.pictureBoxMobileNetSsd = new System.Windows.Forms.PictureBox();
            this.pictureBoxYoloV3 = new System.Windows.Forms.PictureBox();
            this.checkBoxCascade = new System.Windows.Forms.CheckBox();
            this.checkBoxMobileNetSsd = new System.Windows.Forms.CheckBox();
            this.checkBoxFPS = new System.Windows.Forms.CheckBox();
            this.checkBoxYoloV3 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCascade)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMobileNetSsd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxYoloV3)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(93, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // pictureBoxOriginal
            // 
            this.pictureBoxOriginal.Location = new System.Drawing.Point(12, 85);
            this.pictureBoxOriginal.Name = "pictureBoxOriginal";
            this.pictureBoxOriginal.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxOriginal.TabIndex = 3;
            this.pictureBoxOriginal.TabStop = false;
            // 
            // pictureBoxCascade
            // 
            this.pictureBoxCascade.Location = new System.Drawing.Point(339, 85);
            this.pictureBoxCascade.Name = "pictureBoxCascade";
            this.pictureBoxCascade.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxCascade.TabIndex = 4;
            this.pictureBoxCascade.TabStop = false;
            // 
            // pictureBoxMobileNetSsd
            // 
            this.pictureBoxMobileNetSsd.Location = new System.Drawing.Point(12, 331);
            this.pictureBoxMobileNetSsd.Name = "pictureBoxMobileNetSsd";
            this.pictureBoxMobileNetSsd.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxMobileNetSsd.TabIndex = 6;
            this.pictureBoxMobileNetSsd.TabStop = false;
            // 
            // pictureBoxYoloV3
            // 
            this.pictureBoxYoloV3.Location = new System.Drawing.Point(338, 331);
            this.pictureBoxYoloV3.Name = "pictureBoxYoloV3";
            this.pictureBoxYoloV3.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxYoloV3.TabIndex = 7;
            this.pictureBoxYoloV3.TabStop = false;
            // 
            // checkBoxCascade
            // 
            this.checkBoxCascade.AutoSize = true;
            this.checkBoxCascade.Checked = true;
            this.checkBoxCascade.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCascade.Location = new System.Drawing.Point(15, 48);
            this.checkBoxCascade.Name = "checkBoxCascade";
            this.checkBoxCascade.Size = new System.Drawing.Size(97, 19);
            this.checkBoxCascade.TabIndex = 8;
            this.checkBoxCascade.Text = "Use Cascades";
            this.checkBoxCascade.UseVisualStyleBackColor = true;
            // 
            // checkBoxMobileNetSsd
            // 
            this.checkBoxMobileNetSsd.AutoSize = true;
            this.checkBoxMobileNetSsd.Checked = true;
            this.checkBoxMobileNetSsd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMobileNetSsd.Location = new System.Drawing.Point(118, 48);
            this.checkBoxMobileNetSsd.Name = "checkBoxMobileNetSsd";
            this.checkBoxMobileNetSsd.Size = new System.Drawing.Size(124, 19);
            this.checkBoxMobileNetSsd.TabIndex = 9;
            this.checkBoxMobileNetSsd.Text = "Use MobileNetSSD";
            this.checkBoxMobileNetSsd.UseVisualStyleBackColor = true;
            // 
            // checkBoxFPS
            // 
            this.checkBoxFPS.AutoSize = true;
            this.checkBoxFPS.Checked = true;
            this.checkBoxFPS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFPS.Location = new System.Drawing.Point(581, 48);
            this.checkBoxFPS.Name = "checkBoxFPS";
            this.checkBoxFPS.Size = new System.Drawing.Size(77, 19);
            this.checkBoxFPS.TabIndex = 11;
            this.checkBoxFPS.Text = "Show FPS";
            this.checkBoxFPS.UseVisualStyleBackColor = true;
            // 
            // checkBoxYoloV3
            // 
            this.checkBoxYoloV3.AutoSize = true;
            this.checkBoxYoloV3.Checked = true;
            this.checkBoxYoloV3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxYoloV3.Location = new System.Drawing.Point(248, 48);
            this.checkBoxYoloV3.Name = "checkBoxYoloV3";
            this.checkBoxYoloV3.Size = new System.Drawing.Size(95, 19);
            this.checkBoxYoloV3.TabIndex = 10;
            this.checkBoxYoloV3.Text = "Use YOLO V3";
            this.checkBoxYoloV3.UseVisualStyleBackColor = true;
            // 
            // FormCarDetectionComparison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 585);
            this.Controls.Add(this.checkBoxFPS);
            this.Controls.Add(this.checkBoxYoloV3);
            this.Controls.Add(this.checkBoxMobileNetSsd);
            this.Controls.Add(this.checkBoxCascade);
            this.Controls.Add(this.pictureBoxYoloV3);
            this.Controls.Add(this.pictureBoxMobileNetSsd);
            this.Controls.Add(this.pictureBoxCascade);
            this.Controls.Add(this.pictureBoxOriginal);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "FormCarDetectionComparison";
            this.Text = "@elbruno - Car Detection using DNN and Cascades";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCascade)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMobileNetSsd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxYoloV3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.PictureBox pictureBoxOriginal;
        private System.Windows.Forms.PictureBox pictureBoxCascade;
        private System.Windows.Forms.PictureBox pictureBoxMobileNetSsd;
        private System.Windows.Forms.PictureBox pictureBoxYoloV3;
        private System.Windows.Forms.CheckBox checkBoxCascade;
        private System.Windows.Forms.CheckBox checkBoxMobileNetSsd;
        private System.Windows.Forms.CheckBox checkBoxFPS;
        private System.Windows.Forms.CheckBox checkBoxYoloV3;
    }
}

