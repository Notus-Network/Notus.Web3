using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Notus.Web3
{
    public class Common
    {
        public static string FindAvailableNode(string UrlText)
        {
            string MainResultStr = string.Empty;
            bool exitInnerLoop = false;
            while (exitInnerLoop == false)
            {
                for (int a = 0; a < Notus.Variable.Constant.ListMainNodeIp.Count && exitInnerLoop == false; a++)
                {
                    try
                    {
                        MainResultStr = GetRequest(
                            MakeHttpListenerPath(
                                Notus.Variable.Constant.ListMainNodeIp[a],
                                Notus.Variable.Constant.PortNo_HttpListener
                            ) + UrlText,
                            10,
                            true
                        );
                    }
                    catch (Exception err)
                    {
                        SleepWithoutBlocking(5, true);
                    }
                    exitInnerLoop = (MainResultStr.Length > 0);
                }
            }
            return MainResultStr;
        }
        public static async Task<string> PostRequest(string UrlAddress, Dictionary<string, string> PostData)
        {
            HttpResponseMessage response = await new HttpClient().PostAsync(UrlAddress, new FormUrlEncodedContent(PostData));
            return await response.Content.ReadAsStringAsync();
        }
        public static string GetRequest(string UrlAddress, int TimeOut = 0, bool UseTimeoutAsSecond = true)
        {
            try
            {
                System.Net.WebRequest webRequest = System.Net.WebRequest.Create(UrlAddress);
                if (TimeOut > 0)
                {
                    if (UseTimeoutAsSecond == true)
                    {
                        webRequest.Timeout = TimeOut * 1000;
                    }
                    else
                    {
                        webRequest.Timeout = TimeOut;
                    }
                }
                using (System.Net.WebResponse response = webRequest.GetResponse())
                {
                    using (System.IO.Stream content = response.GetResponseStream())
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(content))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return string.Empty;
        }
        public static void Sleep(int SleepTime, bool UseAsSecond = false)
        {
            SleepWithoutBlocking(SleepTime, UseAsSecond);
        }
        public static void SleepWithoutBlocking(int SleepTime, bool UseAsSecond = false)
        {
            DateTime NextTime = (UseAsSecond == true ? DateTime.Now.AddMilliseconds(SleepTime) : DateTime.Now.AddSeconds(SleepTime));
            while (NextTime > DateTime.Now)
            {

            }
        }
        public static string MakeHttpListenerPath(string IpAddress, int PortNo, bool UseSSL = false)
        {
            return (UseSSL == true ? "https" : "http") + "://" + IpAddress + ":" + PortNo + "/";
        }
    }
}
