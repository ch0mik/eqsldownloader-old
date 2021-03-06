﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace eQSL_Downloader
{
    public partial class frmConfig : Form
    {
        public int sliderValue;
        ComponentResourceManager rm;


        public frmConfig(int SliderValue = 5, string lang = null)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(lang);
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            }

            InitializeComponent();
            slider.Value = SliderValue;
            rm = new ComponentResourceManager(this.GetType());
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            lblSlider.Text = new StringBuilder().AppendFormat("{0} {1} {2}", rm.GetString("Config.Opoznienie_w_pobieraniu_eQSL"), slider.Value, rm.GetString("Config.Sekund")).ToString();
            this.Text = rm.GetString("$this.Text");
            btnOK.Text = rm.GetString("Config.btnOK.Text");
            this.Show();
        }

        private void slider_Scroll(object sender, EventArgs e)
        {
            lblSlider.Text = new StringBuilder().AppendFormat("{0} {1} {2}", rm.GetString("Config.Opoznienie_w_pobieraniu_eQSL"), slider.Value, rm.GetString("Config.Sekund")).ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sliderValue = slider.Value;
            this.Close();
        }
    }
}
