
namespace Demo07_WinFormFaceDetectionCascades
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
            this.btnFDCascades = new System.Windows.Forms.Button();
            this.pictureBoxWebCam = new System.Windows.Forms.PictureBox();
            this.pictureBoxEffect = new System.Windows.Forms.PictureBox();
            this.buttonFPS = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWebCam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEffect)).BeginInit();
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
            // btnFDCascades
            // 
            this.btnFDCascades.Location = new System.Drawing.Point(338, 12);
            this.btnFDCascades.Name = "btnFDCascades";
            this.btnFDCascades.Size = new System.Drawing.Size(177, 23);
            this.btnFDCascades.TabIndex = 2;
            this.btnFDCascades.Text = "Face Detection Cascades";
            this.btnFDCascades.UseVisualStyleBackColor = true;
            this.btnFDCascades.Click += new System.EventHandler(this.btnFaceDetectionCascades_Click);
            // 
            // pictureBoxWebCam
            // 
            this.pictureBoxWebCam.Location = new System.Drawing.Point(12, 41);
            this.pictureBoxWebCam.Name = "pictureBoxWebCam";
            this.pictureBoxWebCam.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxWebCam.TabIndex = 3;
            this.pictureBoxWebCam.TabStop = false;
            // 
            // pictureBoxEffect
            // 
            this.pictureBoxEffect.Location = new System.Drawing.Point(338, 41);
            this.pictureBoxEffect.Name = "pictureBoxEffect";
            this.pictureBoxEffect.Size = new System.Drawing.Size(320, 240);
            this.pictureBoxEffect.TabIndex = 4;
            this.pictureBoxEffect.TabStop = false;
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 294);
            this.Controls.Add(this.buttonFPS);
            this.Controls.Add(this.pictureBoxEffect);
            this.Controls.Add(this.pictureBoxWebCam);
            this.Controls.Add(this.btnFDCascades);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "@elbruno - Face Detection using Cascades";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWebCam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEffect)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnFDCascades;
        private System.Windows.Forms.PictureBox pictureBoxWebCam;
        private System.Windows.Forms.PictureBox pictureBoxEffect;
        private System.Windows.Forms.Button buttonFPS;
    }
}

