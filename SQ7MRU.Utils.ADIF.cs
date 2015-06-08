using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace SQ7MRU.Utils.ADIF
{
    public class ADIFReader : IDisposable
    {
        public bool IsDisposed { get; protected set; }
        public bool isInitialized;
        private List<ADIFRow> ADIFRows;
        public string Warnings;
        private string filepath;

        public ADIFReader(String FilePath)
        {
            filepath = FilePath;
            isInitialized = true;
        }
        
        public List<ADIFRow> GetADIFRows()
        {
            string[] rawrecords = ADIFRecords();
            ADIFRows = new List<ADIFRow>();

            foreach (string record in rawrecords)
            {

                ADIFRow adifrow = new ADIFRow();

                string[] x = Regex.Split(record.Replace("\n", "").Replace("\r", ""), @"<([^:]+):\d+[^>]*>").ToArray();
                List<string> l = new List<string>(x);
                l.RemoveAt(0);
                x = l.ToArray();
                
                var dic = new Dictionary<string, string>();
                if (x.Length % 2 == 0)
                {
                    for (int i = 0; i < x.Length; i++)
                    {
                        dic.Add(x[i].ToLower(), x[i + 1]);
                        i++;
                    }


                    PropertyInfo[] props = adifrow.GetType().GetProperties();

                    foreach (PropertyInfo prp in props)
                    {
                        if (dic.ContainsKey(prp.Name.ToLower()))
                        {
                            PropertyInfo pi = typeof(ADIFRow).GetProperty(prp.Name);
                            pi.SetValue(adifrow, dic[prp.Name.ToLower()], null);
                        }

                    }
                }

                if (!string.IsNullOrEmpty(adifrow.call))
                {
                    ADIFRows.Add(adifrow);
                }
            }


            return ADIFRows;
        }


        private string[] ADIFRecords()
        {
            return Extensions.RawRecords(filepath);
     
        }

       

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Do actual Dispose.
        /// </summary>
        /// <remarks>See MSDN doc for implementing the Dispose pattern: Implementing a Dispose Method</remarks>
        /// <param name="disposing">if true, called from managed user code; if false, called from within GC finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                // if true, dispose managed AND unmanaged resources
                if (disposing)
                {
                    // kod
                }
                IsDisposed = true;
            }
        }

    }

    public class ADIFRow
    {
        public string a_index { get; set; }
        public string ant_az { get; set; }
        public string ant_el { get; set; }
        public string band { get; set; }
        public string call { get; set; }
        public string comment { get; set; }
        public string cont { get; set; }
        public string country { get; set; }
        public string cqz { get; set; }
        public string distance { get; set; }
        public string dxcc { get; set; }
        public string eqsl_qslsdate { get; set; }
        public string eqsl_qsl_rcvd { get; set; }
        public string eqsl_qsl_sent { get; set; }
        public string eqsl_status { get; set; }
        public string force_init { get; set; }
        public string freq { get; set; }
        public string gridsquare { get; set; }
        public string app_hamradiodeluxe_heading { get; set; }
        public string ituz { get; set; }
        public string k_index { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string lotw_qslsdate { get; set; }
        public string lotw_qsl_rcvd { get; set; }
        public string lotw_qsl_sent { get; set; }
        public string mode { get; set; }
        public string my_lat { get; set; }
        public string my_lon { get; set; }
        public string name { get; set; }
        public string pfx { get; set; }
        public string qsl_rcvd { get; set; }
        public string qsl_sent { get; set; }
        public string qso_complete { get; set; }
        public string qso_random { get; set; }
        public string qth { get; set; }
        public string rst_rcvd { get; set; }
        public string rst_sent { get; set; }
        public string rx_pwr { get; set; }
        public string sfi { get; set; }
        public string swl { get; set; }
        public string time_off { get; set; }
        public string time_on { get; set; }
        public string qso_date { get; set; }

    }


    internal static class Extensions
    {

        internal static String LoadFileToString(string FilePath)
        {

            using (StreamReader streamReader = new StreamReader(FilePath))
            {
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                return text;
            }
           
        }

        internal static void SaveStringToFile(string Body, string FileName, string Path = null)
        {

            string path;

            if (string.IsNullOrEmpty(Path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory.ToString();
            }
            else
            {
                path = Path;
            }

            using (StreamWriter sw = new StreamWriter(path + FileName))
            {
                sw.Write(Body);
            }
        }

        internal static string XML2JSON(XmlDocument xml)
        {
            return JsonConvert.SerializeXmlNode(xml);
        }


        internal static string XML2JSON(String xmlFilePath)
        {
            XmlDocument _xml = new XmlDocument();
            _xml.Load(xmlFilePath);
            return JsonConvert.SerializeXmlNode(_xml);
        }

        internal static string[] RawRecords(string FilePath)
        {
            string adif = Extensions.LoadFileToString(FilePath);

            string[] RawRecords = adif.Split(new string[] { "<EOH>" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new string[] { "<EOR>" }, StringSplitOptions.RemoveEmptyEntries);

            return RawRecords;
        }

        internal static List<string> ColumnsFromRawRecords(string[] RawRecords)
        {
            List<string> columns = new List<string>();

            for (int i = 0; i < RawRecords.Length; i++)
            {

                string[] kolumna = Regex.Split(RawRecords[1].Replace("\r\n", ""), @"<(.*?):.*?>([^<\t\n\r\f\v]+)").Where(S => !string.IsNullOrEmpty(S)).ToArray();
                for (int ii = 0; ii < kolumna.Length; ii++)
                {
                    if (kolumna.Length % 2 == 0)
                    {
                        if (!columns.Contains(kolumna[ii]))
                        {
                            columns.Add(kolumna[ii]);
                        }
                        ii++;
                    }
                }
            }

            return columns;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }


        internal static string ConvertStringToFormattedDateTime(string QSODateTimeOn, string FormatIn = "yyyyMMddHHmm", string FormatOut = "yyyy-MM-dd HH:mm")
        {
            string _datetimeconverted = QSODateTimeOn;

            if (!(string.IsNullOrEmpty(QSODateTimeOn)))
            {
                try
                {
                    _datetimeconverted = DateTime.ParseExact(QSODateTimeOn.Replace(" ", ""), FormatIn, null).ToString(FormatOut);
                }
                catch (Exception e)
                {
                    _datetimeconverted = QSODateTimeOn;
                }
            }

            return _datetimeconverted;
        }

    }

}
