namespace eQSL_Downloader
{
    partial class frmConfig
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
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.lblSlider = new System.Windows.Forms.Label();
            this.slider = new System.Windows.Forms.TrackBar();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.slider)).BeginInit();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(254, 96);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // lblSlider
            // 
            this.lblSlider.AutoSize = true;
            this.lblSlider.Location = new System.Drawing.Point(12, 9);
            this.lblSlider.Name = "lblSlider";
            this.lblSlider.Size = new System.Drawing.Size(200, 13);
            this.lblSlider.TabIndex = 1;
            this.lblSlider.Text = "Opóźnienie w pobieraniu eQSL 0 sekund";
            // 
            // slider
            // 
            this.slider.Location = new System.Drawing.Point(260, 0);
            this.slider.Name = "slider";
            this.slider.Size = new System.Drawing.Size(325, 45);
            this.slider.TabIndex = 2;
            this.slider.Scroll += new System.EventHandler(this.slider_Scroll);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(476, 61);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(103, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Zatwierdź";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 96);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.slider);
            this.Controls.Add(this.lblSlider);
            this.Controls.Add(this.splitter1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
            this.Text = "Konfiguracja";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.slider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label lblSlider;
        private System.Windows.Forms.TrackBar slider;
        private System.Windows.Forms.Button btnOK;
    }
}