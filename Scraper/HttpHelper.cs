using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    internal static class HttpHelper
    {
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
                Console.WriteLine(ex.Message);
                throw;
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
