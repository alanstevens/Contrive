using System.IO;
using System.Net.Mail;
using System.Web;
using Contrive.Core;

namespace Contrive.Web
{
  public class EmailService : IEmailService
  {
    public EmailService(HttpServerUtilityBase server)
    {
      _server = server;
    }

    readonly HttpServerUtilityBase _server;

    public void SendEmail(string fromAddress, string toAddress, string subject, string messageBody)
    {
      var mm = new MailMessage(fromAddress, toAddress, subject, messageBody);

      new SmtpClient().Send(mm);
    }

    public string BuildMessageBody(string userName, string token, string rootUrl, int timeSpanInHours, string filePath)
    {
      var file = new FileInfo(_server.MapPath(filePath));
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