using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using System.Threading;
using SQ7MRU.Utils.ADIF;


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
                    string URL = "http://eqsl.cc/qslcard/LoginFinish.cfm";
                    string PostData = "Callsign=" + login + "&EnteredPassword=" + password + "&Login=Go";
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    response = wc.UploadString(URL, PostData);
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

        public void Logon(string CallSign)
        {

            using (WebClient wc = new WebClientEx(m_container))
            {
                string URL = "http://eqsl.cc/qslcard/LoginFinish.cfm";
                string PostData = "Callsign=" + CallSign + "&EnteredPassword=" + password + "&Login=Go";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                response = wc.UploadString(URL, PostData);
            }
        }

        public List<CallAndQTH> getCallAndQTH()
        {

            using (WebClient wc = new WebClientEx(m_container))
            {
                string URL = "http://eqsl.cc/qslcard/MyAccounts.cfm";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                response = wc.DownloadString(URL);

                if (saveLog)
                {
                    Extensions.SaveStringToFile(response.Replace(password, "--cut--"),"MyAccounts.htm",path);
                }

                response = response.Replace("<BR><STRONG>Primary</STRONG>", "");
                
                if (response.Contains("You currently have no other accounts attached."))
                {
                    CallAndQTH callqth = new CallAndQTH();
                    callqth.CallSign = login;
                    callqth.QTH = "";
                    CallAndQTHList.Add(callqth);
                    callqth = null;
                }
                else
                {
                    string[] CallAndQTHArray = Regex.Split(response, @"<TD\b[^>]*>(.*?)\<BR\>\((.*?)\)</TD>\r\n").Where(S => S.Length < 50).ToArray();

                    if (CallAndQTHArray.Length % 2 == 0)
                    {
                        

                            for (int i = 0; i < CallAndQTHArray.Length; i++)
                            {
                                CallAndQTH callqth = new CallAndQTH();
                                callqth.CallSign = CallAndQTHArray[i];
                                callqth.QTH = CallAndQTHArray[i + 1];
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
            string ADIF ="", URL = "";
            
            using (WebClient wc = new WebClientEx(m_container))
            {
                if (!string.IsNullOrEmpty(callqth.QTH))
                {
                    URL = "http://www.eqsl.cc/qslcard/DownloadInBox.cfm?UserName=" + callqth.CallSign + "&Password=" + password + "&QTHNickname=" + callqth.QTH ;
                }
                else
                {
                    URL = "http://www.eqsl.cc/qslcard/DownloadInBox.cfm?UserName=" + callqth.CallSign + "&Password=" + password;
                }


                try
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    response = wc.DownloadString(URL);

                    if (saveLog)
                    {
                        Extensions.SaveStringToFile(response.Replace(password, "--cut--"),"DownloadInBox_" + callqth.CallSign.Replace("/", "_") + ".htm", path);
                    }

                    wc.DownloadFile("http://eqsl.cc/qslcard/" + Regex.Match(response, @"((downloadedfiles\/)+[\w\d:#@%/;$()~_?\+-=\\\.&]*)").ToString(), path + callqth.CallSign.Replace("/", "_") + "-" + Regex.Replace(callqth.QTH, patternAlphaNumeric, "") + ".ADIF");
                }
                catch (Exception exc)
                {
                    error += Environment.NewLine + exc.Message;
                }

             }

            return ADIF;

        }

        public List<string> getUrlsFromADIFFile(string ADIFpath)
        {
            List<string> Urls = new List<string>();


                ADIFReader ar = new ADIFReader(ADIFpath);
                List<ADIFRow> ADIFRows = ar.GetADIFRows();

                string UrlPrefix = "http://eqsl.cc/qslcard/DisplayeQSL.cfm?";

                foreach (ADIFRow adifrow in ADIFRows)
                {
                    Urls.Add(UrlPrefix + "Callsign=" + adifrow.call + "&VisitorCallsign=" + login + "&QSODate=" + ConvertStringQSODateTimeOnToFormattedDateTime(adifrow.qso_date + adifrow.time_on).Replace(" ", "%20") + ":00.0&Band=" + adifrow.band + "&Mode=" + adifrow.mode);
                }

            return Urls;
        }

        public bool checkLoginWithCallsingFromADIF(string ADIFpath)
        {
            bool check = false;

           string ADIF = Extensions.LoadFileToString(ADIFpath);

            if(ADIF.ToLower().Contains("received eqsls for " + login.ToLower()))
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

            using (ADIFReader ar = new ADIFReader(path + callqth.CallSign.Replace("/", "_") + "-" + Regex.Replace(callqth.QTH, patternAlphaNumeric, "") + ".ADIF"))
            {
                
                string UrlPrefix = "http://eqsl.cc/qslcard/DisplayeQSL.cfm?";
                List<string> Urls = new List<string>();
                List<ADIFRow> ADIFRows = ar.GetADIFRows();
                foreach (ADIFRow adifrow in ADIFRows)
                {
                    try
                    {
                        Urls.Add(UrlPrefix + "Callsign=" + adifrow.call + "&VisitorCallsign=" + callqth.CallSign + "&QSODate=" + ConvertStringQSODateTimeOnToFormattedDateTime(adifrow.qso_date + adifrow.time_on).Replace(" ", "%20") + ":00.0&Band=" + adifrow.band + "&Mode=" + adifrow.mode);
                    }
                    catch (Exception exc)
                    {
                        error += Environment.NewLine + exc.Message;
                    }
                }
                
                return Urls;
            }
        }


        //private List<string> HTMLtoList(string ArchiveHTML)
        //{
        //    List<string> UrlList = Regex.Split(ArchiveHTML, @"((DisplayeQSL)+[\w\d:#@%/;$()~_?\+-=\\\.&]*)").Where(S => S.Contains("DisplayeQSL.cfm")).ToList();
        //    return UrlList;

        //}


        public void GetJPGfromURL(string URL, int SleepTime = 0, string Subfolder = null)
        {
            string pathfile = path + FilenameFromURL(URL);

            if (!string.IsNullOrEmpty(Subfolder))
            {
                CreateCallsingSubFolder(Subfolder.Replace("/", "_"));
                pathfile = path + Subfolder.Replace("/", "_") + @"\" + FilenameFromURL(URL);
            }
           

            if (!File.Exists(pathfile) ||new FileInfo(pathfile).Length == 0)
            {
                Thread.Sleep(SleepTime);
                
                using (WebClient wc = new WebClientEx(m_container))
               {
                   wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                   response = wc.DownloadString(URL);
                   wc.DownloadFile("http://eqsl.cc" + Regex.Match(response, @"((\/CFFileServlet\/_cf_image\/)+[\w\d:#@%/;$()~_?\+-=\\\.&]*)").ToString(), pathfile);
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

        public void NewThreadOfGetJPGfromURL(string Url, int SleepTime = 0, string Subfolder = null)
        {
                var t = new Thread(() => GetJPGfromURL(Url,SleepTime,Subfolder));
                t.Start();
                t.Join();
        }

        public string FilenameFromURL(string URL)
        {
            Dictionary<string, string> dic = UrlHelper.Decode(URL).Replace("http://eqsl.cc/qslcard/DisplayeQSL.cfm?", "").Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(s => s.Split('=')[0].ToLower(), s => s.Split('=')[1].ToUpper());
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


        // tips from http://stackoverflow.com/questions/1777221/using-cookiecontainer-with-webclient-class
        public class WebClientEx : WebClient
        {
            public WebClientEx(CookieContainer container)
            {
                this.container = container;
            }

            private readonly CookieContainer container = new CookieContainer();

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

        public class CallAndQTH
        {
            public string CallSign { get; set; }
            public string QTH { get; set; } 
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
}