﻿using SQ7MRU.Utils.eQSL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

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
            this.txtCallsign.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckKeys);
        }

        private void CheckKeys(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btnGO_Click(sender, e);
            }
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
