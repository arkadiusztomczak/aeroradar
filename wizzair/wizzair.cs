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

namespace AeroRadar
{
    class Wizzair
    {
        private string getApiVersion()
        {
            System.Net.WebClient webclient = new System.Net.WebClient();

            System.Net.HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://wizzair.com/static_fe/metadata.json");


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

            httpWebRequest.CookieContainer = new CookieContainer();

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
        //public Flights
        public async Task wizzairFlyFromAsync(string iata, DateTime timeFrom, int timeOffset = 24, int priceLimit = -1)
        {
            List<string> connections = getIatas(iata);
            foreach(string c in connections)
            {
                Console.WriteLine(c);
            }
            Flights ret = new Flights();

            var values = new Dictionary<string, string>
            {
                    { "adultCount", "1" },
                    { "childCount", "0" },
                    { "infantCount", "0" },
                    { "isFlightChange", "false" },
                    { "isSeniorOrStudent", "false" },
                    { "wdc", "true" },
                    { "flightList", "{arrivalStation: 'KUT', departureDate: '2020-06-03', departureStation: 'POZ'}"}
                    //{ "flightList", ""}
            };

            var baseAddress = new Uri(getApiVersion() + "/search/wizzDiscountClub");
            var content = new FormUrlEncodedContent(values);

            //HttpClient client = new HttpClient();

           
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    Console.WriteLine("TEST");
                    //client.DefaultRequestHeaders.Add(":authority", "be.wizzair.com");
                    //client.DefaultRequestHeaders.Add(":method", "POST");
                    //client.DefaultRequestHeaders.Add(":path", getApiVersion() + "/search/search");
                    //client.DefaultRequestHeaders.Add(":scheme", "https");
                    client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                    client.DefaultRequestHeaders.Add("accept-language", "pl-PL,pl;q=0.9,en-US;q=0.8,en;q=0.7");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("content-type", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("origin", "https://be.wizzair.com");
                    client.DefaultRequestHeaders.Add("referer", "https://be.wizzair.com/");
                    client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                    client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                    client.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36");

                    //cookieContainer.Add(baseAddress, new Cookie("_hjid=cc598ad7-fa70-4d5d-83ca-0f827ca93085", "_gcl_au=1.1.596812303.1566330180; _ga=GA1.2.2070352834.1566330181; ASP.NET_SessionId=nim2bbabaqwhkrli0bziuxzj; bm_sz=835DE1888072993F6D7EBAF987000C84~YAAQNcITAupUcIxxAQAAfFb0vQd4mKNI2l5uz70ZVePN5znH6yxtd/mLDC4DgwC/3WuXqNe9IqjdJGKvTN6qsPnBo3B5hUGUJG1vmRMcMVKozEhrdm0T4v8OlE9F7AKss/Wzpl6DCqYKzhn/y+Ml7C+xur8LrNFd+A2W7v4dG7jf8e4y5b3WvcZHOlL/qS8o; ak_bmsc=758CD5F8B54D1712C47F5DD04D0B11960213C235D13300008F69A75ED9E1D109~plIWpVGlUBmIQJymoppFb1fyAwnYtOOLI1SGtAQyjBrFW+gI+dvZ5NiRHRM+muyhk7istJfeItuhfoHXjsA+tLP7c/Eteqyo2PLUt0yrZKw0D3Bwn83IrzEfWChlt2zevDFlmoiLUviueg/+dN/UjSvS94TkzX1h5t8PY/3EYH+5Sthpxw66yZ2FFlJ2W1HXFoQ/QK7itIyfKGXIsisgAyTC8giiCgs3bSnvYIIjV6Cd91zvJDHtA0WoLkQAKe4+jey9b+eNP/b4ss5X+Zt7EHziHwf3ZiM+EITNqH0RY3Lyr3VUwCT6Z4q1mPCg+/PdiRYBahPY9Kv/moPKikmJNrTw==; _gid=GA1.2.60911039.1588029842; RequestVerificationToken=225814f6aeab42409f664958f6364a06; _gat_gtag_UA_2629375_25=1; bm_sv=EE527ABF400F44D0111BE0817C4F80AE~jAuO1DKwhpxwhdqWU6PU9J8xw2X5puw85HK+T8tbr2DgNBY66CHSTCTwcx/JF90HvdQTJR1ndvPWMuhwBw2R0XdEPssEK/63iiVuqOnab651bl3wIuzyTNY3a0PKu4VoKpc8Bl1l+y6vX+WROEEGeBEbuCsQaXFU5lfC5aVMsoA=; _abck=99582EDE4F043F526F0229FFE9F34FFC~-1~YAAQZpgRYIzO94RxAQAAdDEdvgNPMLBDVdFUZfypZ2UEJk6iXZgNdZ5UVoJXOHGGfQLWlgFV7TihBTVSZ6F9USkR+LytZ0XVbhwkxQ2XLeqYViCkmNgQkk8eBIxYz0+32XEjsQB1GN4xipOpVbBurC75Thyg/IM+hIxQOe+PRTOLAfVoQ/HcT7yWVkHnCoKPu+6bFB9wQ83pvKdfOXTKtZxziAHUL7Qjl0Mrr3BFO7ACxn/jP8pfg22bvzRXsTkjvxNUv8qNILRNqYTg6U58v8JOPtnB6gjwd94mHiXEYpHNeDBXfuQSqx66FsxPAn2S/oMj9aHWiBBeum5xsNA40qQemQoo0FfPXPhm5Q==~0~-1~-1"));

                    cookieContainer.Add(baseAddress, new Cookie("_hjid", "cc598ad7-fa70-4d5d-83ca-0f827ca93085"));
                    cookieContainer.Add(baseAddress, new Cookie("_gcl_au", "1.1.596812303.1566330180"));
                    cookieContainer.Add(baseAddress, new Cookie("_ga", "GA1.2.2070352834.1566330181"));
                    cookieContainer.Add(baseAddress, new Cookie("ASP.NET_SessionId", "nim2bbabaqwhkrli0bziuxzj"));
                    cookieContainer.Add(baseAddress, new Cookie("bm_sz", "835DE1888072993F6D7EBAF987000C84~YAAQNcITAupUcIxxAQAAfFb0vQd4mKNI2l5uz70ZVePN5znH6yxtdmLDC4DgwC/3WuXqNe9IqjdJGKvTN6qsPnBo3B5hUGUJG1vmRMcMVKozEhrdm0T4v8OlE9F7AKss/Wzpl6DCqYKzhn/y+Ml7C+xur8LrNFd+A2W7v4dG7jf8e4y5b3WvcZHOlL/qS8o"));
                    cookieContainer.Add(baseAddress, new Cookie("ak_bmsc", "758CD5F8B54D1712C47F5DD04D0B11960213C235D13300008F69A75ED9E1D109~plIWpVGlUBmIQJymoppFb1fyAwnYtOOLI1SGtAQyjBrFW+gI+dvZ5NiRHRM+muyhk7istJfeItuhfoHXjsA+tLP7c/Eteqyo2PLUt0yrZKw0D3Bwn83IrzEfWChlt2zevDFlmoiLUviueg/+dN/UjSvS94TkzX1h5t8PY/3EYH+5Sthpxw66yZ2FFlJ2W1HXFoQQK7itIyfKGXIsisgAyTC8giiCgs3bSnvYIIjV6Cd91zvJDHtA0WoLkQAKe4+jey9b+eNP/b4ss5X+Zt7EHziHwf3ZiM+EITNqH0RY3Lyr3VUwCT6Z4q1mPCg+/PdiRYBahPY9Kv/moPKikmJNrTw"));
                    cookieContainer.Add(baseAddress, new Cookie("_gid", "GA1.2.60911039.1588029842"));
                    cookieContainer.Add(baseAddress, new Cookie("RequestVerificationToken", "225814f6aeab42409f664958f6364a06"));
                    cookieContainer.Add(baseAddress, new Cookie("_gat_gtag_UA_2629375_25", "1"));
                    cookieContainer.Add(baseAddress, new Cookie("bm_sv", "EE527ABF400F44D0111BE0817C4F80AE~jAuO1DKwhpxwhdqWU6PU9J8xw2X5puw85HK+T8tbr2DgNBY66CHSTCTwcxJF90HvdQTJR1ndvPWMuhwBw2R0XdEPssEK/63iiVuqOnab651bl3wIuzyTNY3a0PKu4VoKpc8Bl1l+y6vX+WROEEGeBEbuCsQaXFU5lfC5aVMsoA"));
                    cookieContainer.Add(baseAddress, new Cookie("_abck", "99582EDE4F043F526F0229FFE9F34FFC~-1~YAAQZpgRYIzO94RxAQAAdDEdvgNPMLBDVdFUZfypZ2UEJk6iXZgNdZ5UVoJXOHGGfQLWlgFV7TihBTVSZ6F9USkR+LytZ0XVbhwkxQ2XLeqYViCkmNgQkk8eBIxYz0+32XEjsQB1GN4xipOpVbBurC75Thyg/IM+hIxQOe+PRTOLAfVoQ/HcT7yWVkHnCoKPu+6bFB9wQ83pvKdfOXTKtZxziAHUL7Qjl0Mrr3BFO7ACxnjP8pfg22bvzRXsTkjvxNUv8qNILRNqYTg6U58v8JOPtnB6gjwd94mHiXEYpHNeDBXfuQSqx66FsxPAn2S/oMj9aHWiBBeum5xsNA40qQemQoo0FfPXPhm5Q"));
                    var result = await client.PostAsync(getApiVersion() + "/search/search", content);
                    result.EnsureSuccessStatusCode();
                    Console.WriteLine("RESP=" + result.ToString());
                }
            }



            //System.Net.HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(getApiVersion() + "/search/search");

            //Console.WriteLine(getApiVersion() + "/search/search");
            //httpWebRequest.Headers["Origin"] = "https://be.wizzair.com";
            //httpWebRequest.Headers["Accept-Language"] = "en-US,en;q=0.8,es;q=0.6";
            //httpWebRequest.Headers["Upgrade-Insecure-Requests"] = "1";
            //httpWebRequest.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            //httpWebRequest.Headers["Cache-Control"] = "max-age=0";
            //httpWebRequest.KeepAlive = true;
            //httpWebRequest.Expect = "";
            //httpWebRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            //httpWebRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            //httpWebRequest.ContentType = "application/json";

            //httpWebRequest.Method = "POST";

            //httpWebRequest.CookieContainer = new CookieContainer();








            /*using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"flightList\":[]}";
                //string json = "{\"flightList\":[{\"departureStation\":\"POZ\",\"arrivalStation\":\"KUT\",\"departureDate\":\"2020-02-26\"},{\"departureStation\":\"KUT\",\"arrivalStation\":\"POZ\",\"departureDate\":\"2020-02-26\"}],\"wdc\":\"true\",\"adultCount\":1,\"childCount\":0,\"infantCount\":0}";

                streamWriter.Write(json);
            }*/



            /*var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine((string)result);
            }*/


            //return ret;

        }
    }
}
