using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using SQ7MRU.Utils.eQSL;
using SQ7MRU.Utils.ADIF;

namespace eQSL_Downloader
{
    public partial class frmMain : Form
    {
        private static string Login, Password, SavingPath, CallSignList, progress, ADIFfilename;
        private Downloader eqsl;
        private List<Downloader.CallAndQTH> CallAndQTHList;
        private Dictionary<Downloader.CallAndQTH, List<string>> GetAllUrls;
        private int sleepSliderValue;
      
        public frmMain()
        {
            InitializeComponent();
            Literals();
            lblInfo.AutoSize = false;
            SavingPath = AppDomain.CurrentDomain.BaseDirectory.ToString();
            sleepSliderValue = 0;

        }

        private void AddInfo(string Message, bool NewLine = true, bool Clear = false)
        {

            if (lblInfo.InvokeRequired)
            {
                lblInfo.BeginInvoke(new Action(delegate
                {
                    AddInfo(Message, NewLine, Clear);
                }));
                return;
            }

            if (Clear) lblInfo.Text = "";
            if (NewLine) lblInfo.Text += Environment.NewLine;

            lblInfo.AppendText(Message);
            lblInfo.SelectionStart = lblInfo.Text.Length;
            lblInfo.ScrollToCaret();
                   
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            this.MinimumSize = this.Size;
            
        }

        private void Literals()
        {

            lblLogin.Text = "Login : ";
            lblPassword.Text = "Hasło : ";
            btnLogin.Text = "Zaloguj mnie";
            btnNext.Text = "Dalej";

        }

        private void ChooseFolder()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory.ToString();
                fbd.ShowDialog();
                SavingPath = fbd.SelectedPath + @"\";
                AddInfo("Wybrano folder zapisu na : " + SavingPath);
            }
        }

        private void ShowLogonPanel(bool show = true)
        {

            if (btnLogin.InvokeRequired)
            {
                btnLogin.BeginInvoke(new Action(delegate
                {
                    ShowLogonPanel(show);
                }));
                return;
            }
            
            if (txtLogin.InvokeRequired)
            {
                txtLogin.BeginInvoke(new Action(delegate
                {
                    ShowLogonPanel(show);
                }));
                return;
            }


            if (txtPassword.InvokeRequired)
            {
                txtPassword.BeginInvoke(new Action(delegate
                {
                    ShowLogonPanel(show);
                }));
                return;
            }

            if (show)
            {
                btnLogin.Enabled = true;
                txtLogin.Enabled = true;
                txtPassword.Enabled = true;
            }
            else
            {
                btnLogin.Enabled = false;
                txtLogin.Enabled = false;
                txtPassword.Enabled = false;
            }

        }

        private void DisableChooseFolder()
        {
            if (mnuMain.InvokeRequired)
            {
                mnuMain.BeginInvoke(new Action(delegate
                {
                    DisableChooseFolder();
                }));
                return;
            }

            mnuMain.Items[0].Enabled = false;
        }

       
        private void ShowWaiting(bool show = true)
        {

            if (imgWait.InvokeRequired)
            {
                imgWait.BeginInvoke(new Action(delegate
                {
                    ShowWaiting(show);
                }));
                return;
            }
            if (btnNext.InvokeRequired)
            {
                btnNext.BeginInvoke(new Action(delegate
                {
                    ShowWaiting(show);
                }));
                return;
            }

            if (show)
            {
                imgWait.Visible = true;
                imgWait.Update();
                btnNext.Enabled = false;
                btnNext.Update();
            }
            else
            {
                imgWait.Visible = false;
                imgWait.Update();
                btnNext.Enabled = true;
                btnNext.Update();
            }
        }

        private void FirstLogon()
        {
            ShowWaiting();
            ShowLogonPanel(false);

            Login = txtLogin.Text;
            Password = txtPassword.Text;

            eqsl = new Downloader(Login, Password, SavingPath,true);
            eqsl.Logon();
            CallAndQTHList = eqsl.getCallAndQTH();

            CallSignList = string.Join(", ", CallAndQTHList.Select(S => S.CallSign).Distinct().ToArray());

            
            if (CallAndQTHList.Count > 0)
            {
                AddInfo("Pobieranie eQSLek będzie dla login(ów) : " + CallSignList, false, true);
                ShowLogonPanel(false);
                mnuMain.Items["importujADIFToolStripMenuItem"].Visible = true;
                progress = "nxtGetADIFs";
                ShowWaiting(false);
            }
            else
            {
                ShowLogonPanel();
                AddInfo("Błąd podczas logowania, nie udało się pobrać żadnego znaku przypisanego do tego konta eQSL.cc");
                string response = eqsl.response;
                runBrowserThread(response);
                
                     
            }
        }

        private void runBrowserThread(string HTML)
        {
            var th = new Thread(() =>
            {
                new frmError(HTML).Show();
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                Console.WriteLine("Natigated to {0}", e.Url);
                Application.ExitThread();   // Stops the thread
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(FirstLogon)).Start();
        }

       
        private void Work()
        {

            switch (progress)
            {
                case "nxtGetADIFs":

                    DisableChooseFolder();
                    ShowWaiting();

                    foreach (Downloader.CallAndQTH callqth in CallAndQTHList)
                    {

                        AddInfo("rozpoczęcie pobierania pliku ADIF dla " + callqth.CallSign + " (QTH : " + callqth.QTH + ")");

                        eqsl.Logon(callqth.CallSign);
                        eqsl.getADIF(callqth);

                        AddInfo(" ... zakończono.", false);

                    }
                    progress = "nxtGetUrlsFromADIFs";
                    ShowWaiting(false);
                    break;

                case "nxtGetUrlsFromADIFs":

                    ShowWaiting();

                    AddInfo("Przekształcam rekordy ADIF na URLe do pobrania ...");

                    GetAllUrls = new Dictionary<Downloader.CallAndQTH, List<string>>();


                    foreach (Downloader.CallAndQTH callqth in CallAndQTHList)
                    {
                        if (!string.IsNullOrEmpty(callqth.QTH))
                        {
                            AddInfo(callqth.CallSign + " ( " + callqth.QTH + " ) : rozpoczęcie konwersji ... ");
                        }
                        else
                        {
                            AddInfo(callqth.CallSign + " : rozpoczęcie konwersji ... ");
                        }

                        try
                        {
                            List<string> urls = eqsl.GetURLs(callqth);
                            GetAllUrls.Add(callqth, urls);
                            AddInfo("zakończono (" + urls.Count.ToString() + ").", false);
                            urls = null;
                        }
                        catch (Exception e)
                        {
                            AddInfo(e.Message);
                        }
                    }

                    
                    AddInfo("Rozpoczęcie procedury pobierania eQSLek ...");
                    
                        foreach (Downloader.CallAndQTH callqth in CallAndQTHList)
                        {

                            eqsl.Logon(callqth.CallSign);

                            List<string> Urls = GetAllUrls[callqth];

                            AddInfo("Rozpoczęto pobieranie eQSLek (" + Urls.Count.ToString() + ") dla " + callqth.CallSign + " ( " + callqth.QTH + " ) ...");
                            try
                            {
                               
                                int Counter = 0;

                                foreach (string Url in Urls)
                                {

                                    //Download single eQSL as New Thread
                                    eqsl.NewThreadOfGetJPGfromURL(Url, sleepSliderValue * 1000);
                                    Counter += 1;
                                    AddInfo(Counter + "/" + Urls.Count + " : " + eqsl.FilenameFromURL(Url));
                                }

                            }
                            catch (Exception e)
                            {
                                AddInfo(e.Message);
                            }
                        }



                    AddInfo("Pobrano wszystkie eQSL dla wszystkich znaków.\nMożesz zakończyć działanie programu.");
                    progress = "nxtXML";
                    ShowWaiting(false);

                    break;

                    case "getEQSLsFromADIF":

                        ShowWaiting();
                        List<string> UrlsFromADIF = eqsl.getUrlsFromADIFFile(ADIFfilename);

                        AddInfo("Rozpoczęto pobieranie eQSLek (" + UrlsFromADIF.Count.ToString() + ") dla " + Login + " ...");
                        try
                        {
                            int Counter = 0;

                            foreach (string Url in UrlsFromADIF)
                            {
                                //Download single eQSL as New Thread
                                eqsl.NewThreadOfGetJPGfromURL(Url, sleepSliderValue*1000);
                                Counter += 1;
                                AddInfo(Counter + "/" + UrlsFromADIF.Count + " : " + eqsl.FilenameFromURL(Url));
                            }

                        }
                        catch (Exception exc)
                        {
                            AddInfo(exc.Message);
                        }
                        progress = "nxtXML";
                        ShowWaiting(false);

                    break;

                    case "nxtXML":

                    ShowWaiting();

                    using (XmlWriter writer = XmlWriter.Create(SavingPath + "eQSL.xml"))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("eQSL");


                        foreach (Downloader.CallAndQTH callqth in CallAndQTHList)
                        {
                            List<string> Urls = GetAllUrls[callqth];
                            
                            foreach (string Url in Urls)
                            {

                                writer.WriteStartElement("QSO");
                                Dictionary<string, string> dic = Downloader.UrlHelper.Decode(Url).Replace("http://eqsl.cc/qslcard/DisplayeQSL.cfm?", "").Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(s => s.Split('=')[0].ToLower(), s => s.Split('=')[1].ToUpper());
                                writer.WriteElementString("callsign", dic["callsign"].ToString());
                                writer.WriteElementString("band", dic["band"].ToString());
                                writer.WriteElementString("mode", dic["mode"].ToString());
                                writer.WriteElementString("qsodate", dic["qsodate"].Replace(":00.0", "").ToString());
                                writer.WriteElementString("filename", eqsl.FilenameFromURL(Url).ToString());
                                dic = null;
                                writer.WriteEndElement();

                            }
                        }
                     
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        AddInfo("Zakończono zapis pliku XML");
                    }

                    MessageBox.Show("Zapraszam do ponownego skorzystania ;)");

                    this.Close();

                    break;

                default:
                    AddInfo("wystąpił błąd ...");
                    break;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {

            new Thread(new ThreadStart(Work)).Start();
        
        }


        private void zmieńFolderZapisu_Click(object sender, EventArgs e)
        {
            ChooseFolder();
        }

  
        private void oProgramie_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void imgSQ7MRU_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://sq7mru.blogspot.com");
        }

        private void importujADIFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "(.ADI*)|*.ADI*";
                ofd.ShowDialog();
                ADIFfilename = ofd.FileName;
                if (!string.IsNullOrEmpty(ADIFfilename))
                {
                    
                    eqsl.Logon(Login);

                    if (eqsl.checkLoginWithCallsingFromADIF(ADIFfilename))
                    {
     
                        progress = "getEQSLsFromADIF";
                        new Thread(new ThreadStart(Work)).Start();

                    }
                    else
                    {
                        AddInfo("Login nie jest ten sam co w pliku ADIF");
                    }
                    
               }
            }

        }

        private void konfiguracjaMenu_Click(object sender, EventArgs e)
        {
            using (frmConfig cfgFrm = new frmConfig(sleepSliderValue))
            {
                cfgFrm.ShowDialog();
                sleepSliderValue = cfgFrm.sliderValue;
                AddInfo("Wybrano opóźnienie w pobieraniu eQSLek na " + sleepSliderValue.ToString() + " sekund");
            }


        }

 


    }
}
