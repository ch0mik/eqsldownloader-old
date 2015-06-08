using System;
using System.Windows.Forms;

namespace eQSL_Downloader
{
    public partial class frmError : Form
    {
        public string html;
        
        public frmError(string HTML)
        {
            InitializeComponent();
            html = HTML;
        }

        private void frmError_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("about:blank");
            webBrowser1.Document.Write(html);
            this.Show();
        }



    }
}
