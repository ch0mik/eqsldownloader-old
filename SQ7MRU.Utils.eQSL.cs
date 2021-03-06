﻿using HtmlAgilityPack;
using SQ7MRU.Utils.ADIF;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace SQ7MRU.Utils.eQSL
{
    public class Downloader
    {
        public readonly CookieContainer m_container;
        private string path, login, password, patternAlphaNumeric;
        public string response, error;
        public List<CallAndQTH> CallAndQTHList;
        public bool saveLog;

        public Downloader(string Login, string Password, string Path = null, bool SaveLog = false)
        {
            patternAlphaNumeric = "[^a-zA-ZæÆøØåÅéÉöÖäÄüÜ-ñÑõÕéÉáÁóÓôÔzżźćńółęąśŻŹĆĄŚĘŁÓŃ _]";
            m_container = new CookieContainer();
            CallAndQTHList = new List<CallAndQTH>();
            saveLog = SaveLog;

            if (string.IsNullOrEmpty(Path))
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\";
            }
            else
            {
                path = Path;
            }
            login = Login;
            password = Password;
        }

        public void Logon()
        {
            try
            {
                using (WebClient wc = new WebClientEx(m_container))
                {
                    string URL = "https://eqsl.cc/qslcard/LoginFinish.cfm";
                    string PostData = "Callsign=" + login + "&EnteredPassword=" + password + "&Login=Go";
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    response = wc.UploadString(URL, PostData);

                    if (response.Contains(@">Select one<"))
                    {
                        string[] QTHNicknamesArray = Regex.Split(response, @"NAME=""HamID"" VALUE=""(.*)""").Where(S => S.Length < 50).ToArray();

                        foreach (var hamid in QTHNicknamesArray)
                        {
                            PostData = "HamID=" + hamid + "&Callsign=" + login + "&EnteredPassword=" + password + "&SelectCallsign=Log+In";
                            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            response = wc.UploadString(URL, PostData);
                        }
                    }

                    if (saveLog)
                    {
                        Extensions.SaveStringToFile(response.Replace(password, "--cut--"), "LoginFinish.htm", path);
                    }
                }
            }
            catch (Exception exc)
            {
                error += error;
            }
        }

        public void Logon(string CallSign, string HamID = null)
        {
            using (WebClient wc = new WebClientEx(m_container))
            {
                string URL = "https://eqsl.cc/qslcard/LoginFinish.cfm";
                string PostData;

                if (!string.IsNullOrEmpty(HamID))
                {
                    PostData = "HamID=" + HamID + "&Callsign=" + CallSign + "&EnteredPassword=" + password + "&SelectCallsign=Log+In";
                }
                else
                {
                    PostData = "Callsign=" + CallSign + "&EnteredPassword=" + password + "&Login=Go";
                }
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                response = wc.UploadString(URL, PostData);
            }
        }

        public List<CallAndQTH> getCallAndQTH()
        {
            using (WebClient wc = new WebClientEx(m_container))
            {
                string URL = "https://eqsl.cc/qslcard/MyAccounts.cfm";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                response = wc.DownloadString(URL);

                if (saveLog)
                {
                    Extensions.SaveStringToFile(response.Replace(password, "--cut--"), "MyAccounts.htm", path);
                }

                response = response.Replace("<BR><STRONG>Primary</STRONG>", "");

                if (response.Contains("You currently have no other accounts attached."))
                {
                    HtmlDocument doc = new HtmlDocument();

                    doc.LoadHtml(response);
                    var forms = doc.DocumentNode.SelectNodes("//form");

                    foreach (var form in forms)
                    {
                        if (form.Attributes["action"].Value == "AttachAccount.cfm")
                        {
                            CallAndQTH callqth = new CallAndQTH();
                            callqth.QTH = "";
                            callqth.CallSign = form.ChildNodes[5]?.ChildNodes[1]?.InnerText;
                            callqth.UserID = form.Elements("input")?.ToArray()[0]?.Attributes["value"]?.Value;
                            callqth.HamID = form.Elements("input")?.ToArray()[1]?.Attributes["value"]?.Value;
                            CallAndQTHList.Add(callqth);
                        }
                    }
                }
                else
                {
                    string[] CallAndQTHArray = Regex.Split(response, @"<TD\b[^>]*>(.*?)\<BR\>\((.*?)\)</TD>\r\n").Where(S => S.Length < 50).ToArray();
                    string[] HamIDArray = Regex.Split(response, @"NAME=""HamID"" VALUE=""(.*)""").Where(S => S.Length < 50).ToArray();

                    if (CallAndQTHArray.Length % 2 == 0)
                    {
                        for (int i = 0; i < CallAndQTHArray.Length; i++)
                        {
                            CallAndQTH callqth = new CallAndQTH();
                            callqth.CallSign = CallAndQTHArray[i];
                            callqth.QTH = CallAndQTHArray[i + 1];
                            callqth.HamID = HamIDArray[i + 1];
                            CallAndQTHList.Add(callqth);
                            callqth = null;
                            i++;
                        }
                    }
                }
                return CallAndQTHList;
            }
        }

        public string getADIF(CallAndQTH callqth)
        {
            string URL = "https://www.eqsl.cc/qslcard/DownloadInBox.cfm?UserName=" + callqth.CallSign + "&Password=" + password;

            Logon(callqth.CallSign, callqth.HamID);

            using (WebClient wc = new WebClientEx(m_container))
            {
                if (!string.IsNullOrEmpty(callqth.QTH))
                {
                    URL += $"&QTHNickname={callqth.QTH}";
                }

                try
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    response = wc.DownloadString(URL);

                    if (saveLog)
                    {
                        Extensions.SaveStringToFile(response.Replace(password, "--cut--"), "DownloadInBox_" + callqth.CallSign.Replace("/", "_") + ".htm", path);
                    }

                    var adifUrl = Regex.Match(response, @"((downloadedfiles\/)+[\w\d:#@%/;$()~_?\+-=\\\.&]*)").ToString();
                    if (!string.IsNullOrEmpty(adifUrl))
                    {
                        var adifFile = Path.Combine(path, $"{callqth.CallSign.Replace("/", "_")}{Regex.Replace(callqth.QTH, patternAlphaNumeric, "")}.ADIF");
                        wc.DownloadFile("https://eqsl.cc/qslcard/" + adifUrl, adifFile);
                        callqth.Adif = adifFile;
                    }
                }
                catch (Exception exc)
                {
                    error += Environment.NewLine + exc.Message;
                }
            }

            return callqth.Adif;
        }

        public List<string> getUrlsFromADIFFile(string ADIFpath)
        {
            List<string> Urls = new List<string>();

            ADIFReader ar = new ADIFReader(ADIFpath);
            List<ADIFRow> ADIFRows = ar.GetAdifRows();

            string UrlPrefix = "https://eqsl.cc/qslcard/DisplayeQSL.cfm?";

            foreach (ADIFRow adifrow in ADIFRows)
            {
                Urls.Add(UrlPrefix + "Callsign=" + adifrow.call + "&VisitorCallsign=" + login + "&QSODate=" + ConvertStringQSODateTimeOnToFormattedDateTime(adifrow.qso_date + adifrow.time_on).Replace(" ", "%20") + ":00.0&Band=" + adifrow.band + "&Mode=" + adifrow.submode ?? adifrow.mode);
            }

            return Urls;
        }

        public bool checkLoginWithCallsingFromADIF(string ADIFpath)
        {
            bool check = false;

            string ADIF = Extensions.LoadFileToString(ADIFpath);

            if (ADIF.ToLower().Contains("received eqsls for " + login.ToLower()))
            { check = true; }

            return check;
        }

        private string ConvertStringQSODateTimeOnToFormattedDateTime(string QSODateTimeOn)
        {
            string _datetimeconverted = QSODateTimeOn;

            if (!(string.IsNullOrEmpty(QSODateTimeOn)))
            {
                try
                {
                    _datetimeconverted = DateTime.ParseExact(QSODateTimeOn.Replace(" ", ""), "yyyyMMddHHmm", null).ToString("yyyy-MM-dd HH:mm");
                }
                catch (Exception e)
                {
                    _datetimeconverted = QSODateTimeOn;
                }
            }

            return _datetimeconverted;
        }

        public List<string> GetURLs(CallAndQTH callqth)
        {
            List<string> Urls = new List<string>();
            if (!string.IsNullOrEmpty(callqth.Adif))
            {
                using (ADIFReader ar = new ADIFReader(callqth.Adif))
                {
                    string UrlPrefix = "https://eqsl.cc/qslcard/DisplayeQSL.cfm?";
                    List<ADIFRow> ADIFRows = ar.GetAdifRows();
                    foreach (ADIFRow adifrow in ADIFRows)
                    {
                        try
                        {
                            Urls.Add(UrlPrefix + "Callsign=" + adifrow.call + "&VisitorCallsign=" + callqth.CallSign + "&QSODate=" + ConvertStringQSODateTimeOnToFormattedDateTime(adifrow.qso_date + adifrow.time_on).Replace(" ", "%20") + ":00.0&Band=" + adifrow.band + "&Mode=" + (adifrow.submode ?? adifrow.mode));
                        }
                        catch (Exception exc)
                        {
                            error += Environment.NewLine + exc.Message;
                        }
                    }
                }
            }
            return Urls;
        }

        public void GetJPGfromURL(string URL, int SleepTime = 5, string Subfolder = null)
        {
            string pathfile = path + FilenameFromURL(URL);

            if (!string.IsNullOrEmpty(Subfolder))
            {
                CreateCallsingSubFolder(Subfolder.Replace("/", "_"));
                pathfile = path + Subfolder.Replace("/", "_") + @"\" + FilenameFromURL(URL);
            }

            if (!File.Exists(pathfile) || new FileInfo(pathfile).Length == 0)
            {
                using (WebClient wc = new WebClientEx(m_container))
                {
                    bool slowDown = true;
                    int _sleepTime = 0;
                    while (slowDown)
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        response = wc.DownloadString(URL);
                        if (response.Contains("There is no entry for a QSO"))
                        {
                            Dictionary<string, string> dic = UrlHelper.Decode(URL).Replace("https://eqsl.cc/qslcard/DisplayeQSL.cfm?", "").Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(s => s.Split('=')[0].ToLower(), s => s.Split('=')[1].ToUpper());
                            var x = CallAndQTHList.Where(S => S.CallSign == dic["visitorcallsign"]).FirstOrDefault();
                        }
                        else if (!response.Contains("Slow down!"))
                        {
                            wc.DownloadFile("https://eqsl.cc" + Regex.Match(response, @"((\/CFFileServlet\/_cf_image\/)+[\w\d:#@%/;$()~_?\+-=\\\.&]*)").ToString(), pathfile);
                            if (File.ReadAllText(pathfile).Contains("<!-- Application In Root eQSL Folder -->"))
                            { File.Delete(pathfile); }
                            else
                            { slowDown = false; }
                        }
                        else
                        {
                            Thread.Sleep(_sleepTime += SleepTime);
                            if (_sleepTime > 6000) { _sleepTime = 0; }
                        }
                    }
                }
            }
        }

        public void NewThreadOfGetJPGfromURL(List<string> Urls)
        {
            foreach (string Url in Urls)
            {
                var t = new Thread(() => GetJPGfromURL(Url));
                t.Start();
            }
        }

        public void NewThreadOfGetJPGfromURL(string Url, int SleepTime = 5, string Subfolder = null)
        {
            var t = new Thread(() => GetJPGfromURL(Url, SleepTime, Subfolder));
            t.Start();
            t.Join();
        }

        public string FilenameFromURL(string URL)
        {
            Dictionary<string, string> dic = UrlHelper.Decode(URL).Replace("https://eqsl.cc/qslcard/DisplayeQSL.cfm?", "").Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(s => s.Split('=')[0].ToLower(), s => s.Split('=')[1].ToUpper());
            string Filename = dic["callsign"].Replace("/", "-") + "_" + dic["qsodate"].Replace(":00.0", "").Replace(":", "").Replace(" ", "").Replace("-", "") + "_" + dic["band"] + "_" + dic["mode"] + ".JPG";
            dic = null;

            return Filename;
        }

        private void CreateCallsingSubFolder(string Callsing)
        {
            bool folderExists = Directory.Exists(path + Callsing);
            if (!folderExists)
                Directory.CreateDirectory(path + Callsing);
        }

        public class CallAndQTH
        {
            public string CallSign { get; set; }
            public string QTH { get; set; }
            public string HamID { get; set; }
            public string UserID { get; set; }
            public string Adif { get; set; }
            public ConcurrentDictionary<ADIFRow, string> QSOs { get; set; }

            public CallAndQTH()
            {
                QSOs = new ConcurrentDictionary<ADIFRow, string>();
            }
        }

        public static class UrlHelper
        {
            public static string Encode(string str)
            {
                var charClass = String.Format("0-9a-zA-Z{0}", Regex.Escape("-_.!~*'()"));
                return Regex.Replace(str,
                    String.Format("[^{0}]", charClass),
                    new MatchEvaluator(EncodeEvaluator));
            }

            public static string EncodeEvaluator(Match match)
            {
                return (match.Value == " ") ? "+" : String.Format("%{0:X2}", Convert.ToInt32(match.Value[0]));
            }

            public static string DecodeEvaluator(Match match)
            {
                return Convert.ToChar(int.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.HexNumber)).ToString();
            }

            public static string Decode(string str)
            {
                return Regex.Replace(str.Replace('+', ' '), "%[0-9a-zA-Z][0-9a-zA-Z]", new MatchEvaluator(DecodeEvaluator));
            }

            public static List<Uri> ConvertListStringToListUri(List<string> UrlList)
            {
                List<Uri> Urls = new List<Uri>();

                foreach (string Url in UrlList)
                {
                    Uri _url = new Uri(Url);
                    Urls.Add(_url);
                    _url = null;
                }

                return Urls;
            }
        }
    }

    public class iQSLHRDLOG
    {
        public readonly CookieContainer m_container;
        private string path, callsign;
        public string response, error;
        public bool saveLog;

        public iQSLHRDLOG(string Callsign, string Path = null, bool SaveLog = false)
        {
            m_container = new CookieContainer();
            saveLog = SaveLog;

            if (string.IsNullOrEmpty(Path))
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\";
            }
            else
            {
                path = Path;
            }
            callsign = Callsign;
        }

        private List<string> HTMLtoList(string HTML)
        {
            List<string> UrlList = Regex.Split(HTML, @"((PrintQsl)+[\w\d:#@%/;$()~_?\+-=\\\.&]*)").Where(S => S.Contains("PrintQsl.aspx?id=")).ToList();
            return UrlList;
        }

        public List<string> GetUrls()
        {
            string _old_string = "PrintQsl.aspx?id=";
            string _new_string = "https://www.hrdlog.net/qsl.aspx?id=";

            try
            {
                using (WebClientEx wc = new WebClientEx(m_container))
                {
                    string POSTDATA = "ctl00$ContentPlaceHolder1$TbCallsign=" + callsign + "&ctl00$ContentPlaceHolder1$CbAllQsl=on";
                    string URL = "https://www.hrdlog.net/searchqso.aspx?log=";
                    wc.Cookies.Add(new Uri(URL), new Cookie("callsign", callsign.ToUpper()));
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    response = wc.UploadString(URL, POSTDATA);

                    List<string> _list = new List<string>();
                    foreach (string str in HTMLtoList(response))
                    {
                        _list.Add(str.Replace(_old_string, _new_string));
                    }

                    return _list;
                }
            }
            catch (Exception exc)
            {
                error += exc.Message;
                return null;
            }
        }

        public void GetJPGfromURL(string URL, int SleepTime = 5, string Subfolder = "iQSL_HRDLOG")
        {
            CreateCallsingSubFolder();

            string pathfile = path + Subfolder + @"\" + FilenameFromURL(URL);

            if (!File.Exists(pathfile) || new FileInfo(pathfile).Length == 0)
            {
                Thread.Sleep(SleepTime);

                using (WebClient wc = new WebClientEx(m_container))
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.DownloadFile(URL, pathfile);
                }
            }
        }

        public void NewThreadOfGetJPGfromURL(List<string> Urls)
        {
            foreach (string Url in Urls)
            {
                var t = new Thread(() => GetJPGfromURL(Url));
                t.Start();
            }
        }

        public void NewThreadOfGetJPGfromURL(string Url, int SleepTime = 5, string Subfolder = "iQSL_HRDLOG")
        {
            var t = new Thread(() => GetJPGfromURL(Url, SleepTime, Subfolder));
            t.Start();
            t.Join();
        }

        public string FilenameFromURL(string URL)
        {
            return URL.ToLower().Replace("https://www.hrdlog.net/qsl.aspx?id=", "") + ".JPG";
        }

        private void CreateCallsingSubFolder()
        {
            bool folderExists = Directory.Exists(path + "iQSL_HRDLOG");
            if (!folderExists)
                Directory.CreateDirectory(path + "iQSL_HRDLOG");
        }
    }

    // tips from https://stackoverflow.com/questions/1777221/using-cookiecontainer-with-webclient-class
    public class WebClientEx : WebClient
    {
        public WebClientEx(CookieContainer container)
        {
            this.container = container;
        }

        private readonly CookieContainer container = new CookieContainer();
        public CookieContainer Cookies { get { return container; } }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = container;
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                container.Add(cookies);
            }
        }
    }
}