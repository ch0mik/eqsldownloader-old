using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eQSL_Downloader
{
    public partial class frmConfig : Form
    {
        public int sliderValue;
        

        public frmConfig(int SliderValue = 0)
        {
            InitializeComponent();
            slider.Value = SliderValue;
            lblSlider.Text = "Opóźnienie w pobieraniu eQSL " + slider.Value.ToString() + " sekund";
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            this.Show();
        }

        private void slider_Scroll(object sender, EventArgs e)
        {
            lblSlider.Text = "Opóźnienie w pobieraniu eQSL " + slider.Value.ToString() + " sekund";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sliderValue = slider.Value;
            this.Close();
        }
    }
}
