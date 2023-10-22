using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApp.Misc.AesHelper
{
    public static class AesHelper
    {
        private static byte[] CreateAesBytes(string str)
        {
            if (str.Length < 1 || str.Length > 16)
            {
                throw new ArgumentException("Exceed byte limit.");
            }

            byte[] bytes = new byte[16];
            Encoding.UTF8.GetBytes(str).CopyTo(bytes, 0);

            return bytes;
        }

        public static string AesEncrypt(string key, string iv, string plainText)
        {
            byte[] keyBytes = CreateAesBytes(key);
            byte[] ivBytes = CreateAesBytes(iv);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var transf = aes.CreateEncryptor(keyBytes, ivBytes);
            byte[] cypher = transf.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            string base64 = Convert.ToBase64String(cypher);
            return WebUtility.UrlEncode(base64);
        }

        public static string AesDecrypt(string key, string iv, string cypherBase64)
        {
            byte[] keyBytes = CreateAesBytes(key);
            byte[] ivBytes = CreateAesBytes(iv);
            byte[] cypher = Convert.FromBase64String(cypherBase64);

            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var transf = aes.CreateDecryptor(keyBytes, ivBytes);
            byte[] plainBytes = transf.TransformFinalBlock(cypher, 0, cypher.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}