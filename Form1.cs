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

namespace AeroRadar
{
    public class Flights
    {
        string destination;
        DateTime departureTime;
        DateTime arrivalTime;
        string carrier;
        string flightNumber;
        double price;
    }
    public partial class Form1 : Form
    {
        public void aeroFly(int priceLimit, List<string> homeAirports, DateTime dateFrom, DateTime dateTo)
        {
            if (dateTo < dateFrom)                   MessageBox.Show("Termin przylotu nie może być wcześniejszy od terminu wylotu", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
           
            else if (homeAirports.Count<=0)          MessageBox.Show("Nie wskazano żadnego lotniska domowego.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                flyFrom("POZ", DateTime.Now);
            }
        }
        public Flights[] flyFrom(string iata, DateTime date, int timeOffset = 24, int priceLimit = -1)
        {
            Wizzair wizzair = new Wizzair();
            wizzair.wizzairFlyFromAsync(iata, date);
            return null;
        }
        public CefSharp.WinForms.ChromiumWebBrowser br;
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.browser.Controls.Remove(br);
            string curDir = Directory.GetCurrentDirectory();
            br = new CefSharp.WinForms.ChromiumWebBrowser(String.Format("file:///{0}/results.html", curDir))
            {
                Dock = DockStyle.Fill,
                Size = new Size(600, 600),
                Location = new Point(200, 200),
            };
            
            this.browser.Controls.Add(br);

            if (!int.TryParse(textBox1.Text, out int price)) MessageBox.Show("Nieprawidłowa wartość ceny maksymalnej.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (price <= 0) MessageBox.Show("Nieprawidłowa wartość ceny maksymalnej.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                DateTime dateFrom = dateTimePicker1.Value.Date;
                DateTime dateTo = dateTimePicker2.Value.Date;
                List<string> homeAirports = new List<string>();
                foreach (object itemChecked in checkedListBox1.CheckedItems)
                {
                    homeAirports.Add(itemChecked.ToString());
                }
                aeroFly(price, homeAirports, dateFrom, dateTo);
            }
            
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
    }
}
