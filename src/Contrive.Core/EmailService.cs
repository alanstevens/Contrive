using System.Net.Mail;

namespace Contrive.Core
{
  public class EmailService : IEmailService
  {
    public void SendEmail(string fromAddress, string toAddress, string subject, string messageBody)
    {
      var mm = new MailMessage(fromAddress, toAddress, subject, messageBody);

      new SmtpClient().Send(mm);
    }
  }
}