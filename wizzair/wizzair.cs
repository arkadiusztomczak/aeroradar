using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using System;
using System.Net.Cache;
using System;
using System.Threading;
using System.Windows.Forms;
namespace AeroRadar
{
    public class SimpleWebProxy : IWebProxy
    {
        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return destination;
        }

        public bool IsBypassed(Uri host)
        {
            // if return true, service will be very slow.
            return false;
        }

        private static SimpleWebProxy defaultProxy = new SimpleWebProxy();
        public static SimpleWebProxy Default
        {
            get
            {
                return defaultProxy;
            }
        }
    }

    class Wizzair
    {
        public static int fnId = 0;
        CookieContainer _cookieJar = new CookieContainer();
        private string getApiVersion()
        {
            System.Net.WebClient webclient = new System.Net.WebClient();

            System.Net.HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://wizzair.com/static_fe/metadata.json");

            httpWebRequest.CookieContainer = _cookieJar;


            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var metaReader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
            {
                string responseText = metaReader.ReadToEnd();
                dynamic metadataResponse = JObject.Parse(responseText);

                return (string)metadataResponse.apiUrl;
            }
        }
        private List<string> getIatas(string flyFrom)
        {
            List<string> result = new List<string>();
            System.Net.HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(getApiVersion() + "/asset/map");
            httpWebRequest.Headers["Origin"] = "https://be.wizzair.com";
            httpWebRequest.Headers["Accept-Language"] = "en-US,en;q=0.8,es;q=0.6";
            httpWebRequest.Headers["Upgrade-Insecure-Requests"] = "1";
            httpWebRequest.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            httpWebRequest.Headers["Cache-Control"] = "max-age=0";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Expect = "";
            httpWebRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            httpWebRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            httpWebRequest.ContentType = "application/json";

            httpWebRequest.CookieContainer = _cookieJar;

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
            {
                string responseText = reader.ReadToEnd();
                var wizzCities = JsonConvert.DeserializeObject<wizzairCities>((string)responseText);
                foreach (City ct in wizzCities.cities)
                {
                    if (ct.iata == flyFrom)
                    {
                        foreach (Connection cn in ct.connections)
                        {
                            result.Add(cn.iata);
                            Console.WriteLine(cn.iata);
                        }
                    }
                }
            }
            return result;
        }
        public void wizz3()
        {
            fnId++;
            int fxId = 0;
            int fiId = fnId;
            Console.WriteLine(fiId.ToString()+". Wizz3");
            string inHtml = "";
            
            //ScrapingBrowser browser = new ScrapingBrowser();
            WebBrowser w = new WebBrowser();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            w.DocumentCompleted += (s, e) =>
            {
                fxId++;
                if (fxId == 7) MessageBox.Show("Finish in "+watch.ElapsedMilliseconds + "ms.","",MessageBoxButtons.OK);
                Console.WriteLine(fiId.ToString() + ". in " + watch.ElapsedMilliseconds + "ms. got "+fxId.ToString()+". result ");
                var wb = (WebBrowser)s;

                var html = wb.Document.GetElementsByTagName("HTML")[0].OuterHtml;
                System.IO.File.WriteAllText(@"C:\xampp\wizzTest"+ fiId+ fxId.ToString() + ".html", html);
                
                //var domd = wb.Document.GetElementById("copyright").InnerText;
                /* ... */
            };
            
            //w.ScriptErrorsSuppressed = true;
            w.Navigate("https://wizzair.com/#/booking/select-flight/WAW/BCN/2020-06-16/null/1/0/0/0/null");

            
        }
        private void Wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
           
        }
        public void wizz2(string iata, string timeFrom, int timeOffset = 24, int priceLimit = -1)
        {
            //HttpWebRequest.DefaultWebProxy = null;

            List<string> connections = getIatas(iata);
            foreach (string c in connections)
            {
                _cookieJar = new CookieContainer();
                System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getApiVersion() + "/search/search");
                request.Method = "POST";


                var jsonContent = new
                {
                    wdc = true,
                    adultCount = 1,
                    childCount = 0,
                    infantCount = 0,
                    flightList = new[]
                    {
                        new{departureStation = iata, arrivalStation = c, departureDate = timeFrom }
                    }
                };

                request.Headers["Origin"] = "https://wizzair.com";
                request.Headers["Referer"] = "https://wizzair.com";
                request.ContentLength = (JsonConvert.SerializeObject(jsonContent)).Length;
                request.Headers["Accept-Language"] = "pl-PL,pl;q=0.9,en-US;q=0.8,en;q=0.7";
                request.Headers["Accept-encoding"] = "gzip, deflate, br";
                request.Headers["Upgrade-Insecure-Requests"] = "1";
                request.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
                request.Headers["Cache-Control"] = "max-age=0";
                request.Headers["x-requestverificationtoken"] = "252738a09ee44a77983f5a99d2984ebb";
                request.Headers["sec-fetch-dest"] = "";
                request.Headers["sec-fetch-mode"] = "cors";
                request.Headers["sec-fetch-site"] = "same-site";
                request.KeepAlive = true;
                request.Expect = "";
                request.Accept = "application/json, text/javascript, */*";
                request.Headers["X-Requested-With"] = "XMLHttpRequest";
                request.ContentType = "application/json";
                request.CookieContainer = _cookieJar;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(jsonContent));
                }

                

                var watch = System.Diagnostics.Stopwatch.StartNew();
                var httpResponse = (HttpWebResponse)request.GetResponse();
                String response = "";
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                watch.Stop();

                Console.WriteLine("Checking flights for " + c + " took " + watch.ElapsedMilliseconds + "ms.");

                dynamic result = JsonConvert.DeserializeObject(response);
                if (response != "")
                    Console.WriteLine("Lot " + iata + " - " + result.outboundFlights[0].arrivalStation + ", wylot: " + result.outboundFlights[0].departureDateTime + ", przylot: " + result.outboundFlights[0].arrivalDateTime + ", cena: " + result.outboundFlights[0].fares[3].discountedPrice.amount);
                else Console.WriteLine("Brak wyniku lub wynik niepewny!");

            }

        }
        public List<resultJson.Flight> wizzairFlights(string iata, string iataTarget, string timeFrom, string nameFrom, string nameTarget, int r = 0)
        {
            List<resultJson.Flight> output = new List<resultJson.Flight>();

            Console.WriteLine("Looking for flights from " + iata + " to " + iataTarget + " on "+timeFrom+"...");
            var clientSearch = new RestClient();


           
            //Console.WriteLine(c);

            _cookieJar = new CookieContainer();
            _cookieJar.Add(new Cookie("RequestVerificationToken", "d59641dca9584717add5b80c8d5b29c6") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("_abck", "CEA2B5881386AF5CAEB4491CF7F809C3~-1~YAAQL6QFF9c8MVByAQAAchzUbANMwBhFNSjKBQAvRFLDk/OLCcwT4vdpuI0a3mEzffhOnu8PYtNcAVYAyhrh6szp7msHk/TgXzP8/5O3uLAL56o031YYhVMkRM88gCKfAfdK6J5MzOsxxsfUD65lFsT9d7+XbncYUyku5oUMMdYJjLB9/T8rU90qb16cW8K92GdlmLoujZfn9mKx3qgaRSE8K6sXSmsSw84SdR5Zg9uiU6lpFJLiQTPUxIYothCv4iBTowy6GFMZajsU9LOivbvHrr2fSf2w+JaN+2Jj7LDwNSz0WZhXeouhEg==~-1~-1~-1") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("_ga", "GA1.2.2010121629.1590926346") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("_gat_gtag_UA_2629375_25", "1") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("_gcl_au", "1.1.1644715599.1590926346") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("_gid", "GA1.2.237166969.1590926346") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("_hjid", "4ba6b923-4568-48b0-96a5-16919d7e5ea5") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("ak_bmsc", "474C91D023CEB11C5A112EDD064172BE1705A42F8F6700001B2ED45E43BA3173~plwb74/YRBUWY1egn0N+NaeW6D/Solew+G7Lo6Eqz5pF/TLuPLNLeCrdtqzc/2m3l3CNOVw7MGsd030/FM4ndG/UygsBXYhFSxJ46IWWR/PtBOn+jC3DtFKxOUB6+pBp3LY14RxDxUZEOu9xjTc8qys7pn9jmyAWC1kpozpUChZ3C0Ksd4L6QrmikA552PprHV8+w1LmV6D/49FKgEpk20ck3hvAdsVcswIypItG2GEbOrMrNlpkymy+qeoVf7vKgyy8a0IQ5UfdtDSIQkpL6kQR/C69POM8L+AnQe1i2wyXKNiRehH8DlzTkJ/uiMqr07tc2yvgqa4qCUKf8iWWz1UQ==") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("bm_mi", "40F77783022B58DDE0B0EAD2F7BA06CC~0VVk2vyShUW4yOYXhPefbnS8S1XE53SnNeopaoeQOPB6HGyrY5ZrnhzmvOFjs+PccqhM+0gj7x7t3oqdWhJhbn+ekLIt+qjb9d1AXop1B77jOqKulnC8l1ykPi5hm2rTexEmrL99MTUui4KNXEH+MinBeXFM3iJMPkR/hUO8Lf4isZ5ATw2R59//xEPVO63eCFpLrA2sVnCU9wXBG4i1EgwRsfeetldwnyDpmLIW7NE=") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("bm_sv", "6346459B1960F7B0E332F7124779AEA5~Gdid4NSbSt+k9hNK7mGGUcqAqnd7MsTQeCa33+Y5qMEajYxUPdvF/WGGs6G8/EZ2gPN5EKNQydRKTb1wF63wwH83N7tVyMwkvPs0jpKK98mmfZ0CwNMlfzHsaGK5CpkJM8qZfCa8sMN+z1o968uc5VvHbUa51BwUpP2tFiZ+zlg=") { Domain = ".wizzair.com" });
            _cookieJar.Add(new Cookie("bm_sz", "F96582B19EBC4B83502283562389BC55~YAAQL6QFF9Y8MVByAQAAchzUbAdMQSxtozzZJZ3fs86WNRtVpxOMP4I5eerlkMZ72gPn/EMqzHg3BIMPgLfsG3myoqB9n21WiLUSG0oLG/7VHlJKD3lGJJEFYc11ZlTbC+sx5grXaF3g9jxCF96CE7yzUjtDTN+aanUEtTquiFv9qVFqu+5Ux0APu5wgc9JJ") { Domain = ".wizzair.com" });



            //var clientBuildnumber = new RestClient("https://wizzair.com/buildnumber");
            /*clientSearch = new RestClient("https://wizzair.com/");
            var requestSearch = new RestRequest(Method.GET);
            clientSearch.CookieContainer = _cookieJar;
            clientSearch.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);*/

            //requestSearch.AddHeader(":authority", "wizzair.com");
            //requestSearch.AddHeader(":method", "GET");
            //requestSearch.AddHeader(":path", "/");
            //requestSearch.AddHeader(":scheme", "https");
            //requestSearch.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            /*requestSearch.AddHeader("accept-encoding", "gzip, deflate, br");
            requestSearch.AddHeader("accept-language", "pl-PL,pl;q=0.9,en-US;q=0.8,en;q=0.7");
            requestSearch.AddHeader("sec-fetch-dest", "document");
            requestSearch.AddHeader("sec-fetch-mode", "navigate");
            requestSearch.AddHeader("sec-fetch-site", "none");
            requestSearch.AddHeader("upgrade-insecure-requests", "1");
            requestSearch.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");

            clientSearch.Execute(requestSearch);

            foreach (Cookie cookie in _cookieJar.GetCookies(new Uri("https://wizzair.com")))
            {
                Console.WriteLine("Name = {0} ; Value = {1} ; Domain = {2}",
                    cookie.Name, cookie.Value, cookie.Domain);
            }
            */
            clientSearch = new RestClient(getApiVersion() + "/search/search");
            //clientSearch.Proxy = null;



            //clientSearch.Proxy = SimpleWebProxy.Default;

            //var requestBuild = new RestRequest(Method.POST);
            //var requestSearch = new RestRequest(Method.OPTIONS);
            //clientSearch.Execute(requestSearch);
            var requestSearch = new RestRequest(Method.POST);



            var jsonContent = new
            {
                wdc = true,
                adultCount = 1,
                childCount = 0,
                infantCount = 0,
                flightList = new[]
            {
                new{departureStation = iata, arrivalStation = iataTarget, departureDate = timeFrom }
                //new{departureStation = "KUT", arrivalStation = "POZ", departureDate = "2020-06-21" }
                }
            };


            //requestBuild.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            /*requestBuild.AddHeader("X-Requested-With", "XMLHttpRequest");
            requestBuild.AddHeader("Content-Type", "application/json");
            requestBuild.AddHeader("Accept-Language", "en-US,en;q=0.8,es;q=0.6");
            requestBuild.AddHeader("Upgrade-Insecure-Requests", "1");
            requestBuild.AddHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36");
            requestBuild.AddHeader("Cache-Control", "max-age=0");
            requestBuild.AddHeader("Connection", "keep-alive");
            requestBuild.AddHeader("Expect", "");
            requestBuild.AddHeader("Origin", "https://be.wizzair.com/");*/

            //requestSearch.RequestFormat = DataFormat.Json;

            //requestSearch.AddJsonBody(jsonContent);
            requestSearch.AddParameter("application/json", JsonConvert.SerializeObject(jsonContent), ParameterType.RequestBody);

            requestSearch.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            requestSearch.AddHeader("X-Requested-With", "XMLHttpRequest");
            requestSearch.AddHeader("Content-Type", "application/json");
            requestSearch.AddHeader("Accept-Language", "en-US,en;q=0.8,es;q=0.6");
            requestSearch.AddHeader("Upgrade-Insecure-Requests", "1");
            requestSearch.AddHeader("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36");
            requestSearch.AddHeader("Cache-Control", "max-age=0");
            requestSearch.AddHeader("Connection", "keep-alive");
            requestSearch.AddHeader("Expect", "");
            requestSearch.AddHeader("Origin", "https://wizzair.com/");
            requestSearch.AddHeader("Referer", "https://wizzair.com/");
            requestSearch.AddHeader("x-requestverificationtoken", "252738a09ee44a77983f5a99d2984ebb");
            requestSearch.Timeout = 100000000;

            //clientBuildnumber.CookieContainer = _cookieJar;

            clientSearch.CookieContainer = _cookieJar;
            //clientSearch.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);

            //if(r==1) clientSearch.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheOnly);
            //clientSearch.Timeout = 8000;

            //IRestResponse responseB = clientBuildnumber.Execute(requestBuild);
            var watch = System.Diagnostics.Stopwatch.StartNew();

            IRestResponse response = clientSearch.Execute(requestSearch);


            //response = clientSearch.PostAsync(requestSearch);
            //IRestResponse response = clientSearch.Execute(requestSearch);
            watch.Stop();

            //Console.WriteLine("Checking flights for " + iataTarget + " took " + watch.ElapsedMilliseconds + "ms:");
            //MessageBox.Show("Checking flights for " + c + " took " + watch.ElapsedMilliseconds + "ms.", "RS", MessageBoxButtons.OK);
            //clientSearch.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheOnly);

            dynamic result = JsonConvert.DeserializeObject(response.Content);
            //Console.WriteLine(response.Content.ToString());
            //Flights output = new AeroRadar.Flights();
            if (response.Content.ToString() != "")
            {
                foreach (Cookie cookie in _cookieJar.GetCookies(new Uri("https://wizzair.com")))
                 {
                     Console.WriteLine("Name = {0} ; Value = {1} ; Domain = {2}",
                         cookie.Name, cookie.Value, cookie.Domain);
                 }
                if (result.message != null)
                {
                    Console.WriteLine(result.messsage.toString());
                    wizzairFlights(iata, iataTarget, timeFrom,nameFrom,nameTarget);

                }
                else {
                    Console.WriteLine(iataTarget + " " + watch.ElapsedMilliseconds + "ms" + ": Flight " + iata + " - " + result.outboundFlights[0].arrivalStation + ", departure: " + result.outboundFlights[0].departureDateTime + ", arrive: " + result.outboundFlights[0].arrivalDateTime + ", price: " + result.outboundFlights[0].fares[3].discountedPrice.amount);

                    resultJson.Flight fl = new resultJson.Flight();
                    resultJson.Route[] flR = new resultJson.Route[1];
                    resultJson.Route flRt = new resultJson.Route();

                    string departureDateTime = result.outboundFlights[0].departureDateTime;
                    string arrivalDateTime = result.outboundFlights[0].arrivalDateTime;
                    string testArrName = (string)result.outboundFlights[0].arrivalStation;
                    //string airportName = Form1.getAirportName((string)result.outboundFlights[0].arrivalStation);

                    fl.beginDate = departureDateTime.Substring(0, 10) + " " + departureDateTime.Substring(11);
                    //fl.destination = Form1.getAirportName((string)result.outboundFlights[0].arrivalStation);
                    fl.destination = nameTarget;
                    fl.destinationCode = result.outboundFlights[0].arrivalStation;
                    fl.endDate = arrivalDateTime.Substring(0, 10) + " " + arrivalDateTime.Substring(11);
                    //fl.origin = Form1.getAirportName((string)result.outboundFlights[0].departureStation);
                    fl.origin = nameFrom;
                    fl.originCode = result.outboundFlights[0].departureStation;
                    fl.price = result.outboundFlights[0].fares[3].discountedPrice.amount;

                    flRt.arrival = fl.endDate;
                    flRt.arriveTo = fl.destination;
                    flRt.arriveToCode = fl.destinationCode;
                    flRt.carrier = "Wizzair";
                    flRt.departure = fl.beginDate;
                    flRt.departureFrom = fl.origin;
                    flRt.departureFromCode = fl.originCode;
                    flRt.flight = result.outboundFlights[0].carrierCode + " " + result.outboundFlights[0].flightNumber;
                    flRt.price = fl.price;

                    flR[0] = flRt;
                    fl.route = flR;

                    output.Add(fl);
                }
            }
            else Console.WriteLine(iataTarget + " " + watch.ElapsedMilliseconds + "ms" + ": No connection found!");

            //wizzProcess(iata, iataTarget, timeFrom,1);

            return output;

        }
        public void wizzairFlyFromAsync(string source, string target, string time)
        {

            //System.Net.WebRequest.DefaultWebProxy = null;
            HttpWebRequest.DefaultWebProxy = null;
            //List<string> connections = getIatas(iata);
            
            var clientSearch = new RestClient();

            wizzairFlights(source, target, time,"","");
 

/*            foreach (string c in connections)
            {
                Thread thread = new Thread(() => wizzProcess(iata,c,timeFrom));
                thread.Start();

            }*/

            

        }
    }
}
