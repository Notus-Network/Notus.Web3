using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Notus.Web3
{
    public class Common
    {
        public static async Task<string> PostRequest(string UrlAddress, Dictionary<string, string> PostData)
        {
            HttpResponseMessage response = await new HttpClient().PostAsync(UrlAddress, new FormUrlEncodedContent(PostData));
            return await response.Content.ReadAsStringAsync();
        }
        public static string MakeHttpListenerPath(string IpAddress, int PortNo, bool UseSSL = false)
        {
            return (UseSSL == true ? "https" : "http") + "://" + IpAddress + ":" + PortNo + "/";
        }
    }
}
