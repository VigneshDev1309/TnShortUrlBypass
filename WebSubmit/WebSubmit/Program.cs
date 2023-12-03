using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using HtmlAgilityPack;
using System.Net;
using Microsoft.SqlServer.Server;
using System.Web;

namespace WebSubmit
{
    internal class Program
    {
        static void Main(string[] args)
        { 
            // Watch https://github.com/VigneshDev1309/TnShortUrlBypass/tree/main#video-demonstration
            // Replace CODE to your shorten link code
            Console.WriteLine(Script("CODE").Result);
            Console.ReadLine();
        }

        static async Task<string> Script(string code)
        {
            string baseUrl = "https://go.tnshort.net/";
            string arg1 = code;
            string arg2 = "links/go";
            string _ref = "ref" + arg1;

            ServicePointManager.Expect100Continue = false;

            Dictionary<string, string> requestHeaders = new Dictionary<string, string>();

            #region requestHeaders one
            requestHeaders.Add("Host", "go.tnshort.net");
            requestHeaders.Add("Connection", "keep-alive");
            requestHeaders.Add("sec-ch-ua", "\"Brave\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
            requestHeaders.Add("sec-ch-ua-mobile", "?0");
            requestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            requestHeaders.Add("Upgrade-Insecure-Requests", "1");
            requestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            requestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
            requestHeaders.Add("Sec-GPC", "1");
            requestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            requestHeaders.Add("Sec-Fetch-Site", "cross-site");
            requestHeaders.Add("Sec-Fetch-Mode", "navigate");
            requestHeaders.Add("Sec-Fetch-User", "?1");
            requestHeaders.Add("Sec-Fetch-Dest", "document");
            requestHeaders.Add("Referer", "https://market.finclub.in/");
            requestHeaders.Add("Cache-Control", "no-cache");
            requestHeaders.Add("Accept-Encoding", "gzip, deflate");
            #endregion

            Dictionary<string, string> response = await Web(baseUrl + arg1, requestHeaders);

            if (response.ContainsKey("Error"))
            {
                return response["Error"];
            }
            else
            {
                CookieContainer cookieContainer = new CookieContainer();
                Dictionary<string, string> parseReponses = Parse(response["Html"]);
                foreach (var parseReponse in parseReponses)
                {
                    if (!response.ContainsKey(parseReponse.Key))
                        response.Add(parseReponse.Key, parseReponse.Value);
                }
                parseReponses.Clear();
                if (response.ContainsKey("ParseError"))
                {
                    return response["ParseError"];
                }
                else
                {
                    #region requestHeaders two
                    requestHeaders.Clear();
                    requestHeaders.Add("Host", "go.tnshort.net");
                    requestHeaders.Add("Connection", "keep-alive");
                    requestHeaders.Add("sec-ch-ua", "\"Brave\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
                    requestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                    requestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    requestHeaders.Add("sec-ch-ua-mobile", "?0");
                    requestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
                    requestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                    requestHeaders.Add("Sec-GPC", "1");
                    requestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                    requestHeaders.Add("Origin", "https://go.tnshort.net");
                    requestHeaders.Add("Sec-Fetch-Site", "same-origin");
                    requestHeaders.Add("Sec-Fetch-Mode", "cors");
                    requestHeaders.Add("Sec-Fetch-Dest", "empty");
                    requestHeaders.Add("Cache-Control", "no-cache");
                    requestHeaders.Add("Referer", "https://go.tnshort.net/vExmPdt");
                    requestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    #endregion
                    var formData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("_method", "POST"),
                        new KeyValuePair<string, string>("_csrfToken", response["csrfToken"]),
                        new KeyValuePair<string, string>("ad_form_data", response["adFormData"]),
                        new KeyValuePair<string, string>("_Token[fields]", response["tokenFields"]),
                        new KeyValuePair<string, string>("_Token[unlocked]", response["tokenUnlocked"])
                    });
                    cookieContainer.Add(new Uri(baseUrl + arg2), new Cookie("csrfToken", response["csrfToken"]));
                    cookieContainer.Add(new Uri(baseUrl + arg2), new Cookie("app_visitor", response["app_visitor"]));
                    cookieContainer.Add(new Uri(baseUrl + arg2), new Cookie("AppSession", response["AppSession"]));
                    cookieContainer.Add(new Uri(baseUrl + arg2), new Cookie(_ref, response[_ref]));
                    cookieContainer.Add(new Uri(baseUrl + arg2), new Cookie("ab", "2"));
                    response = await Web(baseUrl + arg2, requestHeaders, formData, cookieContainer);
                    if (response.ContainsKey("Error"))
                    {
                        return response["Error"];
                    }
                    else
                    {
                        return response["Html"]
                            .Replace("{\"status\":\"success\",\"message\":\"\",\"url\":\"", string.Empty)
                            .Replace("\"}", string.Empty)
                            .Replace("\\", string.Empty);
                    }
                }
            }
        }

        static Dictionary<string, string> Parse(string response)
        {
            if(string.IsNullOrEmpty(response)) return null;

            Dictionary<string, string> datas = new Dictionary<string, string>();

            var document = new HtmlDocument();
            document.LoadHtml(response);

            var csrfToken = document.DocumentNode.SelectSingleNode("//input[@name='_csrfToken']")?.GetAttributeValue("value", "");
            var adFormData = document.DocumentNode.SelectSingleNode("//input[@name='ad_form_data']")?.GetAttributeValue("value", "");
            var tokenFields = document.DocumentNode.SelectSingleNode("//input[@name='_Token[fields]']")?.GetAttributeValue("value", "");
            var tokenUnlocked = document.DocumentNode.SelectSingleNode("//input[@name='_Token[unlocked]']")?.GetAttributeValue("value", "");


            if(csrfToken!=null && adFormData !=null && tokenFields !=null && tokenUnlocked!=null) 
            {
                datas.Add("csrfToken", csrfToken);
                datas.Add("adFormData", adFormData);
                datas.Add("tokenFields", tokenFields);
                datas.Add("tokenUnlocked", tokenUnlocked);
            }
            else
            {
                datas.Add("ParseError", "Missing expected data");
            }
            return datas;
        }

        static async Task<Dictionary<string, string>> Web(string url, 
            Dictionary<string, string> requestHeaders, 
            FormUrlEncodedContent form = null,
            CookieContainer cookieContainer = null)
        {
            Dictionary<string, string> datas = new Dictionary<string, string>();
            HttpClientHandler handler = new HttpClientHandler();
            using (HttpClient client = new HttpClient(handler))
            {
                if(cookieContainer != null)
                    handler.CookieContainer = cookieContainer;
                if (requestHeaders.Count() > 0)
                {
                    client.DefaultRequestHeaders.Clear();
                    foreach (var item in requestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                try
                {
                    HttpResponseMessage response = null;
                    if (form == null)
                    {
                        response = await client.GetAsync(url);
                    }
                    else
                    {
                        response = await client.PostAsync(url, form);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        await Task.Delay(3000);
                        if (form == null)
                        {
                            foreach (var header in response.Headers.GetValues("Set-Cookie"))
                            {
                                var cookieParts = header.Split(';')[0].Split('=');
                                if (cookieParts.Length == 2)
                                {
                                    datas.Add(cookieParts[0], cookieParts[1]);
                                }
                            }
                        }
                        if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                        {
                            using (Stream decompressedStream = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                            {
                                using (StreamReader reader = new StreamReader(decompressedStream))
                                {
                                    datas.Add("Html", reader.ReadToEnd());
                                }
                            }
                        }
                        else if (response.Content.Headers.ContentEncoding.Contains("deflate"))
                        {
                            using (Stream decompressedStream = new DeflateStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                            {
                                using (StreamReader reader = new StreamReader(decompressedStream))
                                {
                                    datas.Add("Html", reader.ReadToEnd());
                                }
                            }
                        }
                        else
                        {
                            datas.Add("Html", await response.Content.ReadAsStringAsync());
                        }
                    }
                    else
                    {
                        datas.Add("Error", $"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    datas.Add("Error", $"{ex.Message}");
                }
            }
            return datas;
        }
    }
}
