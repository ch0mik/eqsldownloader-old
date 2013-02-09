namespace eQSL_Downloader
{
    partial class frmHRDLOG
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
            this.btnGO = new System.Windows.Forms.Button();
            this.txtCallsign = new System.Windows.Forms.TextBox();
            this.lblCallsing = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnGO
            // 
            this.btnGO.Location = new System.Drawing.Point(4, 38);
            this.btnGO.Name = "btnGO";
            this.btnGO.Size = new System.Drawing.Size(198, 33);
            this.btnGO.TabIndex = 0;
            this.btnGO.Text = "GO";
            this.btnGO.UseVisualStyleBackColor = true;
            this.btnGO.Click += new System.EventHandler(this.btnGO_Click);
            // 
            // txtCallsign
            // 
            this.txtCallsign.Location = new System.Drawing.Point(110, 12);
            this.txtCallsign.Name = "txtCallsign";
            this.txtCallsign.Size = new System.Drawing.Size(92, 20);
            this.txtCallsign.TabIndex = 1;
            // 
            // lblCallsing
            // 
            this.lblCallsing.AutoSize = true;
            this.lblCallsing.Location = new System.Drawing.Point(12, 15);
            this.lblCallsing.Name = "lblCallsing";
            this.lblCallsing.Size = new System.Drawing.Size(88, 13);
            this.lblCallsing.TabIndex = 2;
            this.lblCallsing.Text = "hrdlog.net login : ";
            // 
            // frmHRDLOG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(207, 75);
            this.Controls.Add(this.lblCallsing);
            this.Controls.Add(this.txtCallsign);
            this.Controls.Add(this.btnGO);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmHRDLOG";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "iQSL Downloader";
            this.Load += new System.EventHandler(this.frmHRDLOG_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGO;
        private System.Windows.Forms.TextBox txtCallsign;
        private System.Windows.Forms.Label lblCallsing;
    }
}