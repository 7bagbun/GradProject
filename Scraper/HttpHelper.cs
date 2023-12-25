using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    internal static class HttpHelper
    {
        public static async Task<bool> SendRequest(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var msg = new HttpRequestMessage(HttpMethod.Post, url);
                    var resp = await client.SendAsync(msg);

                    return resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                return false;
            }
        }

        public static async Task<byte[]> DownloadImageBytesAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var resp = await client.GetAsync(url))
                    {
                        return await resp.Content.ReadAsByteArrayAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                return new byte[0];
            }
        }

        public static string UrlEncodedStringToBase64(string str)
        {
            str = System.Net.WebUtility.UrlEncode(str);
            var bytes = Encoding.UTF8.GetBytes(str);
            string res = Convert.ToBase64String(bytes);

            return res;
        }
    }
}
