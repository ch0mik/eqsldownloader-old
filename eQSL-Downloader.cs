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
using Microsoft.Win32;
using SQ7MRU.Utils.eQSL;
using SQ7MRU.Utils.ADIF;

namespace eQSL_Downloader
{
    public partial class frmMain : Form
    {
        private static string Login, Password, SavingPath, CallSignList, progress, ADIFfilename, lang;
        private Downloader eqsl;
        private List<Downloader.CallAndQTH> CallAndQTHList;
        private Dictionary<Downloader.CallAndQTH, List<string>> GetAllUrls;
        private int sleepSliderValue;
        private ComponentResourceManager rm;

        public frmMain()
        {
            lang = null;
            rm = new ComponentResourceManager(this.GetType());
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            InitializeComponent();
            Literals();
            lblInfo.AutoSize = false;
            SavingPath = readPathFromRegistry();
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

        private void EndProgram()
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(delegate
                {
                    EndProgram();
                }));
                return;
            }

            this.Close();

        }

        private void Literals()
        {
            foreach (ToolStripItem mnitem in mnuMain.Items)
            {
                mnitem.Text = rm.GetString(mnitem.Name + ".Text");
            }

            językToolStripMenuItem.Text = rm.GetString("językToolStripMenuItem.Text");
            zmieńFolderZapisuToolStripMenuItem1.Text = rm.GetString("zmieńFolderZapisuToolStripMenuItem1.Text");
            konfiguracjaToolStripMenuItem.Text = rm.GetString("konfiguracjaToolStripMenuItem.Text");


            foreach(Control c in this.Controls)
            {
                c.Text = rm.GetString(c.Name + ".Text");
            }

           

        }

        private void ChooseFolder()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                
                fbd.SelectedPath = SavingPath;
                fbd.ShowDialog();
                
                SavingPath = fbd.SelectedPath;

                if (SavingPath.Substring(SavingPath.Length - 1) != @"\")
                {
                    SavingPath += @"\";
                }
                savePathToRegistry(SavingPath);
                AddInfo(rm.GetString("str_ChooseFolder") + SavingPath);
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

            if (!string.IsNullOrEmpty(lang))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(lang);
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            }

            ShowWaiting();
            ShowLogonPanel(false);

            Login = txtLogin.Text;
            Password = txtPassword.Text;

            try
            {
                eqsl = new Downloader(Login, Password, SavingPath);
                eqsl.Logon();
                CallAndQTHList = eqsl.getCallAndQTH();

                CallSignList = string.Join(", ", CallAndQTHList.Select(S => S.CallSign).Distinct().ToArray());


                if (CallAndQTHList.Count > 0)
                {
                    AddInfo(rm.GetString("str_InfoCallSignList") + CallSignList, false, true);
                    ShowLogonPanel(false);
                    mnuMain.Items["importujADIFToolStripMenuItem"].Visible = true;
                    progress = "nxtGetADIFs";
                    ShowWaiting(false);
                }
                else
                {
                    ShowLogonPanel();
                    AddInfo(rm.GetString("str_ErrorDuringLogon"));
                    string response = eqsl.response;
                    runBrowserThread(response);


                }
            }
            catch (Exception exc)
            {
                AddInfo(exc.Message);
                ShowLogonPanel();
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
                Application.ExitThread();   // Stops the thread
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(FirstLogon)).Start();
        }

       
        private void Work()
        {

            if (!string.IsNullOrEmpty(lang))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(lang);
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            }

            switch (progress)
            {
                case "nxtGetADIFs":

                    DisableChooseFolder();
                    ShowWaiting();

                    foreach (Downloader.CallAndQTH callqth in CallAndQTHList)
                    {

                        AddInfo(rm.GetString("str_beginDownloadADIFfor") + callqth.CallSign + " (QTH : " + callqth.QTH + ")");

                        eqsl.Logon(callqth.CallSign);
                        eqsl.getADIF(callqth);

                        AddInfo(rm.GetString("str_Done"), false);

                    }
                    progress = "nxtGetUrlsFromADIFs";
                    ShowWaiting(false);
                    break;

                case "nxtGetUrlsFromADIFs":

                    ShowWaiting();

                    AddInfo(rm.GetString("str_ConvertADIFtoURLs"));

                    GetAllUrls = new Dictionary<Downloader.CallAndQTH, List<string>>();


                    foreach (Downloader.CallAndQTH callqth in CallAndQTHList)
                    {
                        if (!string.IsNullOrEmpty(callqth.QTH))
                        {
                            AddInfo(callqth.CallSign + " ( " + callqth.QTH + " ) : " + rm.GetString("str_BeginConvert"));
                        }
                        else
                        {
                            AddInfo(callqth.CallSign + " : " + rm.GetString("str_BeginConvert"));
                        }

                        try
                        {
                            List<string> urls = eqsl.GetURLs(callqth);
                            GetAllUrls.Add(callqth, urls);
                            AddInfo(rm.GetString("str_Done") + " (" + urls.Count.ToString() + ").", false);
                            urls = null;
                        }
                        catch (Exception e)
                        {
                            AddInfo(e.Message);
                        }
                    }


                    AddInfo(rm.GetString("str_BeginDownloadProcedure"));



                        foreach (Downloader.CallAndQTH callqth in CallAndQTHList)
                        {

                            eqsl.Logon(callqth.CallSign,callqth.HamID);
                            if (GetAllUrls.ContainsKey(callqth))
                            {
                                List<string> Urls = GetAllUrls[callqth];

                                AddInfo(rm.GetString("str_BeginDownload_eQSLs") + " (" + Urls.Count.ToString() + ") " + rm.GetString("str_for") + " " + callqth.CallSign + " ( " + callqth.QTH + " ) ...");
                                try
                                {

                                    int Counter = 0;

                                    foreach (string Url in Urls)
                                    {
                                        eqsl.NewThreadOfGetJPGfromURL(Url, sleepSliderValue * 1000, callqth.CallSign);
                                        Counter += 1;
                                        AddInfo(Counter + "/" + Urls.Count + " : " + eqsl.FilenameFromURL(Url));
                                    }

                                }
                                catch (Exception e)
                                {
                                    AddInfo(e.Message);
                                }
                            }

                        }



                    AddInfo(rm.GetString("str_Download_eQSLs_done"));
                    progress = "nxtXML";
                    ShowWaiting(false);

                    break;

                    case "getEQSLsFromADIF":

                        ShowWaiting();
                        List<string> UrlsFromADIF = eqsl.getUrlsFromADIFFile(ADIFfilename);

                        AddInfo(rm.GetString("str_BeginDownload_eQSLs") + " (" + UrlsFromADIF.Count.ToString() + ") " + rm.GetString("str_for") + " " + Login + " ...");
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

                    GenerateXMLandGallery();

                    MessageBox.Show(rm.GetString("str_EndingMessage"));

                    EndProgram();

                    break;

                default:
                    AddInfo(rm.GetString("str_ErrorOccurred"));
                    break;
            }
        }

        
        private void GenerateXMLandGallery(string GalleryName = null)
        {

            using (XmlWriter writer = XmlWriter.Create(SavingPath + "eQSL.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("eQSL");

                //
                foreach (string callsign in CallAndQTHList.Select(S => S.CallSign).Distinct().ToArray())
                {
                    writer.WriteStartElement("Callsign");
                    writer.WriteAttributeString("value", callsign);
               
                    string[] files = Directory.GetFiles(SavingPath + callsign.Replace("/","_") + "\\", "*.JPG");

                    foreach (string file in files)
                    {

                        writer.WriteStartElement("QSO");
                        writer.WriteElementString("filename", file.Replace(SavingPath, ""));

                        string description = "";
                        try 
                        {
                            string[] tempArr = file.Replace(SavingPath + callsign.Replace("/", "_") + "\\", "").Replace(".JPG","").Split(new char[] { '_' });
                            description += tempArr[0].Replace("-", "/") + ", ";
                            description += "QSO UTC Time : " + Extensions.ConvertStringToFormattedDateTime(tempArr[1]) + ", ";
                            description += "Band : " + tempArr[2] + ", ";
                            description += "Mode : " + tempArr[3];
                        }
                        catch(Exception exc){}

                        writer.WriteElementString("description", description);
                        
                        writer.WriteEndElement();

                    }

                    writer.WriteEndElement();

                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Extensions.SaveStringToFile(Extensions.XML2JSON(SavingPath + "eQSL.xml"), "eQSL.json", SavingPath);
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
            new About(lang).ShowDialog();
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
                        AddInfo(rm.GetString("str_BadLoginInADIF"));
                    }
                    
               }
            }

        }

        private void konfiguracjaMenu_Click(object sender, EventArgs e)
        {
            using (frmConfig cfgFrm = new frmConfig(sleepSliderValue, lang))
            {
                cfgFrm.ShowDialog();
                sleepSliderValue = cfgFrm.sliderValue;
                AddInfo(rm.GetString("str_ChooseDownloadDelay") + sleepSliderValue.ToString() + rm.GetString("str_Seconds"));
            }


        }

        private void polskiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lang = null;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Literals();
            this.Update();
        }

        private void angielskiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lang = "en-US";
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-US");
            Literals();
            this.Update();
        }


        private string readPathFromRegistry()
        {
            try
            {
                return (string)Registry.CurrentUser.OpenSubKey(@"Software\SQ7MRU\eQSLDownloader").GetValue("SavingPath");
            }
            catch (Exception exc) 
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\";
            }
        }

        private void savePathToRegistry(string savingPath)
        {
            Registry.CurrentUser.CreateSubKey(@"Software\SQ7MRU\eQSLDownloader").SetValue("SavingPath", savingPath);
        }

        private void iQSL_Work()
        {
            List<string> Urls;

            using (frmHRDLOG frmiQSL = new frmHRDLOG(txtLogin.Text, lang))
            {
                frmiQSL.ShowDialog();
                Urls = frmiQSL.Urls;
                string hrdloglogin = frmiQSL.callsign;

                if (Urls.Count > 0)
                {
                    AddInfo(rm.GetString("str_iQSLtoDownload") + Urls.Count.ToString());
                    AddInfo(rm.GetString("str_BeginDownload_iQSL"));

                    try
                    {
                        iQSLHRDLOG iqsl = new iQSLHRDLOG(hrdloglogin, SavingPath);

                        int Counter = 0;

                        foreach (string Url in Urls)
                        {
                            iqsl.NewThreadOfGetJPGfromURL(Url, sleepSliderValue * 1000);
                            Counter += 1;
                            AddInfo(Counter + "/" + Urls.Count + " : " + iqsl.FilenameFromURL(Url));
                        }

                        AddInfo(rm.GetString("str_Done")    );       

                        iqsl = null;
                    }
                    catch (Exception exc)
                    {
                        AddInfo(exc.Message);
                    }
                }
                else
                {
                    AddInfo(rm.GetString("str_iQSLisEmpty"));
                }

            }
        }

        private void iQSLHRDLOGnetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(iQSL_Work)).Start();
        }

    }
}
