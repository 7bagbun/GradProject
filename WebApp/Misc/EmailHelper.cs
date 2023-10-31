using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebApp.Misc
{
    public class EmailHelper
    {
        private const string MAIL_SERVER = "smtp.gmail.com";
        private const string MAIL_ACCOUNT = "iiihomeappliances@gmail.com";
        private readonly string _secret_path;

        public EmailHelper(string path)
        {
            _secret_path = path;
        }

        public void SendEmail(MailMessage msg, MailAddress addr)
        {
            try
            {
                msg.From = new MailAddress(MAIL_ACCOUNT);
                msg.To.Add(addr);

                using (SmtpClient client = new SmtpClient(MAIL_SERVER, 587))
                {
                    var sm = new SecretManager(_secret_path);

                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential(
                        ConfigurationManager.AppSettings["GmailAccount"],
                        sm.GetSecret("gmailPassword"));

                    client.Send(msg);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}