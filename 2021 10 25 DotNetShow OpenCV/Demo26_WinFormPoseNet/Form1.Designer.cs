
namespace Demo12_WinFormPoseNet
{
    partial class Form1
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
            this.btnShowPose = new System.Windows.Forms.Button();
            this.pictureBoxWebCam = new System.Windows.Forms.PictureBox();
            this.buttonFPS = new System.Windows.Forms.Button();
            this.lblOutputAnalysis = new System.Windows.Forms.Label();
            this.btnShowJoints = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWebCam)).BeginInit();
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
            // btnShowPose
            // 
            this.btnShowPose.Location = new System.Drawing.Point(12, 41);
            this.btnShowPose.Name = "btnShowPose";
            this.btnShowPose.Size = new System.Drawing.Size(156, 23);
            this.btnShowPose.TabIndex = 2;
            this.btnShowPose.Text = "Detect Pose";
            this.btnShowPose.UseVisualStyleBackColor = true;
            this.btnShowPose.Click += new System.EventHandler(this.btnDetectPose_Click);
            // 
            // pictureBoxWebCam
            // 
            this.pictureBoxWebCam.Location = new System.Drawing.Point(12, 70);
            this.pictureBoxWebCam.Name = "pictureBoxWebCam";
            this.pictureBoxWebCam.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxWebCam.TabIndex = 3;
            this.pictureBoxWebCam.TabStop = false;
            // 
            // buttonFPS
            // 
            this.buttonFPS.Location = new System.Drawing.Point(257, 12);
            this.buttonFPS.Name = "buttonFPS";
            this.buttonFPS.Size = new System.Drawing.Size(75, 23);
            this.buttonFPS.TabIndex = 5;
            this.buttonFPS.Text = "FPS";
            this.buttonFPS.UseVisualStyleBackColor = true;
            this.buttonFPS.Click += new System.EventHandler(this.buttonFPS_Click);
            // 
            // lblOutputAnalysis
            // 
            this.lblOutputAnalysis.AutoSize = true;
            this.lblOutputAnalysis.Location = new System.Drawing.Point(12, 323);
            this.lblOutputAnalysis.Name = "lblOutputAnalysis";
            this.lblOutputAnalysis.Size = new System.Drawing.Size(101, 15);
            this.lblOutputAnalysis.TabIndex = 6;
            this.lblOutputAnalysis.Text = "lblOutputAnalysis";
            // 
            // btnShowJoints
            // 
            this.btnShowJoints.Location = new System.Drawing.Point(176, 41);
            this.btnShowJoints.Name = "btnShowJoints";
            this.btnShowJoints.Size = new System.Drawing.Size(156, 23);
            this.btnShowJoints.TabIndex = 7;
            this.btnShowJoints.Text = "Show Joints";
            this.btnShowJoints.UseVisualStyleBackColor = true;
            this.btnShowJoints.Click += new System.EventHandler(this.btnShowJoints_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 358);
            this.Controls.Add(this.btnShowJoints);
            this.Controls.Add(this.lblOutputAnalysis);
            this.Controls.Add(this.buttonFPS);
            this.Controls.Add(this.pictureBoxWebCam);
            this.Controls.Add(this.btnShowPose);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "@elbruno - Pose Detector";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWebCam)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnShowPose;
        private System.Windows.Forms.PictureBox pictureBoxWebCam;
        private System.Windows.Forms.Button buttonFPS;
        private System.Windows.Forms.Label lblOutputAnalysis;
        private System.Windows.Forms.Button btnShowJoints;
    }
}

