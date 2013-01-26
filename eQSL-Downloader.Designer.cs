namespace eQSL_Downloader
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblLogin = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.RichTextBox();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.zmieńFolderZapisuToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.konfiguracjaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.językToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polskiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.angielskiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oProgramieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importujADIFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zmieńFolderZapisuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imgSQ7MRU = new System.Windows.Forms.PictureBox();
            this.imgWait = new System.Windows.Forms.PictureBox();
            this.mnuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgSQ7MRU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgWait)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLogin
            // 
            resources.ApplyResources(this.lblLogin, "lblLogin");
            this.lblLogin.Name = "lblLogin";
            // 
            // lblPassword
            // 
            resources.ApplyResources(this.lblPassword, "lblPassword");
            this.lblPassword.Name = "lblPassword";
            // 
            // btnLogin
            // 
            resources.ApplyResources(this.btnLogin, "btnLogin");
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtLogin
            // 
            resources.ApplyResources(this.txtLogin, "txtLogin");
            this.txtLogin.Name = "txtLogin";
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            // 
            // btnNext
            // 
            resources.ApplyResources(this.btnNext, "btnNext");
            this.btnNext.Name = "btnNext";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblInfo
            // 
            resources.ApplyResources(this.lblInfo, "lblInfo");
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.ReadOnly = true;
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem1,
            this.oProgramieToolStripMenuItem,
            this.importujADIFToolStripMenuItem});
            resources.ApplyResources(this.mnuMain, "mnuMain");
            this.mnuMain.Name = "mnuMain";
            // 
            // menuToolStripMenuItem1
            // 
            this.menuToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zmieńFolderZapisuToolStripMenuItem1,
            this.konfiguracjaToolStripMenuItem,
            this.językToolStripMenuItem});
            this.menuToolStripMenuItem1.Name = "menuToolStripMenuItem1";
            resources.ApplyResources(this.menuToolStripMenuItem1, "menuToolStripMenuItem1");
            // 
            // zmieńFolderZapisuToolStripMenuItem1
            // 
            this.zmieńFolderZapisuToolStripMenuItem1.Name = "zmieńFolderZapisuToolStripMenuItem1";
            resources.ApplyResources(this.zmieńFolderZapisuToolStripMenuItem1, "zmieńFolderZapisuToolStripMenuItem1");
            this.zmieńFolderZapisuToolStripMenuItem1.Click += new System.EventHandler(this.zmieńFolderZapisu_Click);
            // 
            // konfiguracjaToolStripMenuItem
            // 
            this.konfiguracjaToolStripMenuItem.Name = "konfiguracjaToolStripMenuItem";
            resources.ApplyResources(this.konfiguracjaToolStripMenuItem, "konfiguracjaToolStripMenuItem");
            this.konfiguracjaToolStripMenuItem.Click += new System.EventHandler(this.konfiguracjaMenu_Click);
            // 
            // językToolStripMenuItem
            // 
            this.językToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.polskiToolStripMenuItem,
            this.angielskiToolStripMenuItem});
            this.językToolStripMenuItem.Name = "językToolStripMenuItem";
            resources.ApplyResources(this.językToolStripMenuItem, "językToolStripMenuItem");
            // 
            // polskiToolStripMenuItem
            // 
            this.polskiToolStripMenuItem.Name = "polskiToolStripMenuItem";
            resources.ApplyResources(this.polskiToolStripMenuItem, "polskiToolStripMenuItem");
            this.polskiToolStripMenuItem.Click += new System.EventHandler(this.polskiToolStripMenuItem_Click);
            // 
            // angielskiToolStripMenuItem
            // 
            this.angielskiToolStripMenuItem.Name = "angielskiToolStripMenuItem";
            resources.ApplyResources(this.angielskiToolStripMenuItem, "angielskiToolStripMenuItem");
            this.angielskiToolStripMenuItem.Click += new System.EventHandler(this.angielskiToolStripMenuItem_Click);
            // 
            // oProgramieToolStripMenuItem
            // 
            this.oProgramieToolStripMenuItem.Name = "oProgramieToolStripMenuItem";
            resources.ApplyResources(this.oProgramieToolStripMenuItem, "oProgramieToolStripMenuItem");
            this.oProgramieToolStripMenuItem.Click += new System.EventHandler(this.oProgramie_Click);
            // 
            // importujADIFToolStripMenuItem
            // 
            this.importujADIFToolStripMenuItem.Name = "importujADIFToolStripMenuItem";
            resources.ApplyResources(this.importujADIFToolStripMenuItem, "importujADIFToolStripMenuItem");
            this.importujADIFToolStripMenuItem.Click += new System.EventHandler(this.importujADIFToolStripMenuItem_Click);
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zmieńFolderZapisuToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            resources.ApplyResources(this.menuToolStripMenuItem, "menuToolStripMenuItem");
            // 
            // zmieńFolderZapisuToolStripMenuItem
            // 
            this.zmieńFolderZapisuToolStripMenuItem.Name = "zmieńFolderZapisuToolStripMenuItem";
            resources.ApplyResources(this.zmieńFolderZapisuToolStripMenuItem, "zmieńFolderZapisuToolStripMenuItem");
            // 
            // imgSQ7MRU
            // 
            this.imgSQ7MRU.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imgSQ7MRU.Image = global::eQSL_Downloader.Properties.Resources.sq7mru;
            resources.ApplyResources(this.imgSQ7MRU, "imgSQ7MRU");
            this.imgSQ7MRU.Name = "imgSQ7MRU";
            this.imgSQ7MRU.TabStop = false;
            this.imgSQ7MRU.Click += new System.EventHandler(this.imgSQ7MRU_Click);
            // 
            // imgWait
            // 
            resources.ApplyResources(this.imgWait, "imgWait");
            this.imgWait.Image = global::eQSL_Downloader.Properties.Resources.progress_bar;
            this.imgWait.Name = "imgWait";
            this.imgWait.TabStop = false;
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.imgSQ7MRU);
            this.Controls.Add(this.imgWait);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.mnuMain);
            this.MainMenuStrip = this.mnuMain;
            this.Name = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgSQ7MRU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgWait)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.RichTextBox lblInfo;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zmieńFolderZapisuToolStripMenuItem;
        private System.Windows.Forms.PictureBox imgWait;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem zmieńFolderZapisuToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem oProgramieToolStripMenuItem;
        private System.Windows.Forms.PictureBox imgSQ7MRU;
        private System.Windows.Forms.ToolStripMenuItem importujADIFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem konfiguracjaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem językToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem polskiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem angielskiToolStripMenuItem;
    }
}

