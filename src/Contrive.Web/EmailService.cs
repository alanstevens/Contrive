using System.IO;
using System.Net.Mail;
using System.Web;
using Contrive.Core;

namespace Contrive.Web
{
  public class EmailService : IEmailService
  {
    readonly HttpServerUtilityBase _server;

    public EmailService(HttpServerUtilityBase server)
    {
      _server = server;
    }

    public void SendEmail(string fromAddress, string toAddress, string subject, string messageBody)
    {
      var mm = new MailMessage(fromAddress, toAddress, subject, messageBody);

      new SmtpClient().Send(mm);
    }

    public string BuildMessageBody(string userName, string token, string filePath)
    {
      string body = "";

      var file = new FileInfo(_server.MapPath(filePath));
      string text = string.Empty;

      if (file.Exists)
      {
        using (StreamReader sr = file.OpenText())
        {
          text = sr.ReadToEnd();
        }
        text = text.Replace("%UserName%", userName);
        text = text.Replace("%Token%", token);
        //text = text.Replace("%RootUrl%", rootUrl);
        //text = text.Replace("%TimeSpan%", timeSpan);
      }
      body = text;

      return body;
    }
  }
}