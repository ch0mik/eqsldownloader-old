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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfig));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.lblSlider = new System.Windows.Forms.Label();
            this.slider = new System.Windows.Forms.TrackBar();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.slider)).BeginInit();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // lblSlider
            // 
            resources.ApplyResources(this.lblSlider, "lblSlider");
            this.lblSlider.Name = "lblSlider";
            // 
            // slider
            // 
            resources.ApplyResources(this.slider, "slider");
            this.slider.Name = "slider";
            this.slider.Value = 5;
            this.slider.Scroll += new System.EventHandler(this.slider_Scroll);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmConfig
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.slider);
            this.Controls.Add(this.lblSlider);
            this.Controls.Add(this.splitter1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
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