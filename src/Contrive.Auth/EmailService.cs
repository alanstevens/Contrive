using System.IO;
using System.Net.Mail;
using Contrive.Common;

namespace Contrive.Auth
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string fromAddress, string toAddress, string subject, string messageBody)
        {
            var mm = new MailMessage(fromAddress, toAddress, subject, messageBody);

            new SmtpClient().Send(mm);
        }

        public string BuildMessageBody(string userName, string token, string rootUrl, int timeSpanInHours, string filePath)
        {
            // TODO: HAS 02/27/2013 Fix the path mapping to work in client and web apps. e.g. Server.MapPath
            var file = new FileInfo(Path.GetFullPath(filePath));
            var text = "";

            if (file.Exists)
            {
                using (var sr = file.OpenText())
                {
                    text = sr.ReadToEnd();
                }
                text = text.Replace("%UserName%", userName);
                text = text.Replace("%Token%", token);
                text = text.Replace("%RootUrl%", rootUrl);
                text = text.Replace("%TimeSpan%", timeSpanInHours.ToString());
            }

            return text;
        }
    }
}