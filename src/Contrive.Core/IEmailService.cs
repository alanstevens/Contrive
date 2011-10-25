namespace Contrive.Core
{
  public interface IEmailService {
    void SendEmail(string fromAddress, string toAddress, string subject, string messageBody);
  }
}