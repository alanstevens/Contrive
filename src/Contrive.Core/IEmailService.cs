namespace Contrive.Core
{
  public interface IEmailService {
    void SendEmail(string fromAddress, string toAddress, string subject, string messageBody);

    string BuildMessageBody(string userName, string token, string filePath);
  }
}