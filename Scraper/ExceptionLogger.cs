using System;
using System.IO;

namespace Scraper
{
    internal static class ExceptionLogger
    {
        public static void LogException(Exception ex)
        {
            using (var sr = File.CreateText("error.log"))
            {
                sr.WriteLine(ex.ToString());
                sr.Flush();
                sr.Close();
            }
        }
    }
}
