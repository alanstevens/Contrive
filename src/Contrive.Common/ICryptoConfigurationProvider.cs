using System;

namespace Contrive.Common
{
  public interface ICryptoConfigurationProvider
  {
    Type EncryptionAlgorithm { get; }

    byte[] EncryptionKey { get; }

    Type HashAlgorithm { get; }

    byte[] HmacKey { get; }
  }
}