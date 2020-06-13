using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.Internals;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace AeroRadar
{
   
    public partial class Form1 : Form
    {
        public class CustomMenuHandler : CefSharp.IContextMenuHandler
        {
            public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                model.Clear();
            }

            public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {

                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {

            }

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
        }
        public CefSharp.WinForms.ChromiumWebBrowser br;
        
        public Form1()
        {
            InitializeComponent();
            /*for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }*/
            string curDir = Directory.GetCurrentDirectory();
            //this.webBrowser1.ScriptErrorsSuppressed = true;
            //this.webBrowser1.Url = new Uri(String.Format("file:///{0}/results.html", curDir));
            //MessageBox.Show(curDir, "TEST", MessageBoxButtons.OK);
            br = new CefSharp.WinForms.ChromiumWebBrowser(String.Format("file:///{0}/blankResults.html", curDir))
            {
                Dock = DockStyle.Fill,
                Size = new Size(600, 600),
                Location = new Point(200, 200),
            };
            br.MenuHandler = new CustomMenuHandler();
            this.browser.Controls.Add(br);
            

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        static string flyFrom;
        static string flyTo;
        static string flyTtime;
        static string airportFrom;
        static string airportTo;
        static int jsP;
        static int jsM;
        static string jsA;
        static string jsI;
        static List<resultJson.Flight> jsResult;

        void jsDataUpdate(int p = -1, int m = -1, string a = "", string i = "")
        {
            if (p != -1) jsP = p;
            if (m != -1) jsM = m;
            if (a != "") jsA = a;
            if (i != "") {
                if(jsI == "") jsI += "'" + i + "'";
                else jsI += ",'" + i + "'";
            }

            string u = "var p = " + jsP + "; var m = " + jsM + "; var a = '" + jsA + "'; var i = [" + jsI + "];";

            File.WriteAllText("progress.js", u);
        }

        public void searchingThread()
        {
            jsP = 0;
            jsM = 0;
            jsA = "";
            jsI = "";

            jsResult = new List<resultJson.Flight>();

            Wizzair wizz = new Wizzair();
            Ryanair ryan = new Ryanair();
            Transavia trans = new Transavia();
            try
            {
                jsDataUpdate(1, 3, "Wizzair", "");
                List<resultJson.Flight> wizzFl = wizz.wizzairFlights(flyFrom, flyTo, flyTtime, airportFrom, airportTo);
                jsResult.AddRange(wizzFl);
            }
            catch(Exception e)
            {
                jsDataUpdate(-1, -1, "", "Nie udało się pobrać danych linii Wizzair!");
            }

            try
            {
                jsDataUpdate(2, 3, "Transavia", "");
                List<resultJson.Flight> transfl = trans.TransaviaSearch(flyFrom,flyTo, flyTtime, airportFrom,airportTo);
                jsResult.AddRange(transfl);
            }
            catch(Exception e)
            {
                jsDataUpdate(-1, -1, "", "Nie udało się pobrać danych linii Transavia!");
            }

            try
            {
                jsDataUpdate(3, 3, "Ryanair", "");
                List<resultJson.Flight> ryanFl = ryan.ryan(flyFrom, flyTo, flyTtime, airportFrom, airportTo);
                jsResult.AddRange(ryanFl);
            }
            catch (Exception e)
            {
                jsDataUpdate(-1, -1, "", "Nie udało się pobrać danych linii Ryanair!");
            }


           



            string toFile = "var flights = JSON.parse('{ \"flights\": [";
            foreach (resultJson.Flight fl in jsResult)
            {
                toFile += (JsonConvert.SerializeObject(fl) + ",");
            }
            toFile = toFile.Remove(toFile.Length - 1);
            toFile += "]}');";
            File.WriteAllText("flights.js", toFile);

            jsDataUpdate(4, 3, "", "");
        }



        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.comboBox1.Text[0] == '[' && this.comboBox1.Text[4] == ']' && this.comboBox2.Text[0] == '[' && this.comboBox2.Text[4] == ']')
                {
                    this.browser.Controls.Remove(br);
                    string curDir = Directory.GetCurrentDirectory();
                    br = new CefSharp.WinForms.ChromiumWebBrowser(String.Format("file:///{0}/results.html", curDir))
                    {
                        Dock = DockStyle.Fill,
                        Size = new Size(600, 600),
                        Location = new Point(200, 200),
                    };
                    br.MenuHandler = new CustomMenuHandler();
                    br.BrowserSettings.FileAccessFromFileUrls = CefState.Enabled;
                    br.BrowserSettings.UniversalAccessFromFileUrls = CefState.Enabled;

                    this.browser.Controls.Add(br);

                    flyFrom = this.comboBox1.Text.Substring(1, 3);
                    flyTo = this.comboBox2.Text.Substring(1, 3);
                    flyTtime = this.dateTimePicker1.Text;
                    airportFrom = (string)this.comboBox1.Text.Substring(5);
                    airportTo = (string)this.comboBox2.Text.Substring(5);

                    Thread searchInAirlines = new Thread(searchingThread);
                    searchInAirlines.Start();






                    //wizz.wizzairFlyFromAsync(flyFrom, flyTo, flyTtimeo);

                    //MessageBox.Show(flyFrom + "-" + flyTo+" on "+ flyTtimeo);

                }
                else MessageBox.Show("Nieprawidłowe lotnisko wylotu lub przylotu. Skorzystaj z lotnisk proponowanych przez formularz!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nieprawidłowe lotnisko wylotu lub przylotu. Skorzystaj z lotnisk proponowanych przez formularz!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


           /* 
            this.browser.Controls.Remove(br);
            string curDir = Directory.GetCurrentDirectory();
            br = new CefSharp.WinForms.ChromiumWebBrowser(String.Format("file:///{0}/results.html", curDir))
            {
                Dock = DockStyle.Fill,
                Size = new Size(600, 600),
                Location = new Point(200, 200),
            };
            
            this.browser.Controls.Add(br);*/

            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public static string getAirportName(string iata)
        {
            string result = iata;

            if (iata.Length == 3)
            {
                WebBrowser w = new WebBrowser();
                w.DocumentCompleted += (s, e) =>
                {
                    var wb = (WebBrowser)s;

                    var table = wb.Document.GetElementsByTagName("tbody");
                    foreach (HtmlElement tab in table)
                    {
                        var rows = tab.GetElementsByTagName("tr");
                        foreach (HtmlElement row in rows)
                        {
                            var cells = row.GetElementsByTagName("td");
                            if (cells[2].InnerText != "Regional" && cells[2].InnerText != "Metropolitan Area")
                            {
                                result = cells[0].InnerText + " " + cells[2].InnerText;
                            }
                        }
                    }
                };
                w.Navigate("https://www.iata.org/AirportCodesSearch/Search?currentBlock=314384&currentPage=12572&airport.search=" + iata);
            };

            return result;
        }

        async void getIatas(string iata, object sender) // 0 - lotnisko + iata, 1 - iata, 2 - lotnisko
        {
            /*string responseText = "";
            System.Net.HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.iata.org/AirportCodesSearch/Search?currentBlock=314384&currentPage=12572&airport.search="+ iata);

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
            {
                responseText = reader.ReadToEnd();
                
            }

            return responseText;*/
            try
            {
                ComboBox c = sender as ComboBox;

                if (iata.Length == 3)
                {
                    await Task.Delay(1000);
                    WebBrowser w = new WebBrowser();
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    w.DocumentCompleted += (s, e) =>
                    {
                        var wb = (WebBrowser)s;

                        var table = wb.Document.GetElementsByTagName("tbody");
                        foreach (HtmlElement tab in table)
                        {
                            //Console.WriteLine("\n\n\n");
                            c.Items.Clear();
                            var rows = tab.GetElementsByTagName("tr");
                            foreach (HtmlElement row in rows)
                            {
                                var cells = row.GetElementsByTagName("td");
                                try
                                {
                                    if (cells[2].InnerText != "Regional" && cells[2].InnerText != "Metropolitan Area")
                                    {
                                        c.Items.Add("[" + cells[3].InnerText + "] " + cells[0].InnerText + " " + cells[2].InnerText);
                                        Console.WriteLine(("[" + cells[3].InnerText + "] " + cells[0].InnerText + " " + cells[2].InnerText));
                                    }
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show("Nie udało się pobrać danych IATA! Sprawdź swoje połączenie z internetem!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            }
                        }
                        c.Select(c.Text.Length, 0);
                        c.DroppedDown = true;
                    };
                    w.Navigate("https://www.iata.org/AirportCodesSearch/Search?currentBlock=314384&currentPage=12572&airport.search=" + iata);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Nie udało się pobrać danych IATA! Sprawdź swoje połączenie z internetem!","Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
        }
         

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;
            //getIatas(t.Text);

            
            
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ComboBox t = sender as ComboBox;
            getIatas(t.Text, sender);
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            ComboBox t = sender as ComboBox;
            getIatas(t.Text, sender);
        }
    }
    public class Flights
    {
        protected string destination;
        public DateTime departureTime;
        public DateTime arrivalTime;
        public string carrier;
        public string flightNumber;
        public double price;
    }
}
