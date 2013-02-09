using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using SQ7MRU.Utils.eQSL;

namespace eQSL_Downloader
{
    public partial class frmHRDLOG : Form
    {
        private string lang;
        public string error, callsign;
        public List<string> Urls;
        ComponentResourceManager rm;

        public frmHRDLOG(string Callsing, string lang = null)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(lang);
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            }


            callsign = Callsing;
            Urls = new List<string>();
            rm = new ComponentResourceManager(this.GetType());
            InitializeComponent();
        }

        private void frmHRDLOG_Load(object sender, EventArgs e)
        {
            txtCallsign.Text = callsign;
            btnGO.Text = rm.GetString("str_btnGO");
            error = "";
        }

 
        private void btnGO_Click(object sender, EventArgs e)
        {
            try
            {
                btnGO.Enabled = false;
                iQSLHRDLOG iqsl = new iQSLHRDLOG(txtCallsign.Text);
                Urls = iqsl.GetUrls();
                callsign= txtCallsign.Text;
            }
            catch (Exception exc)
            {
                error += exc.Message;
            }
            finally
            {
                btnGO.Enabled = true;
                this.Close();
            }
        }
    }
}
