using System;

namespace Contrive.Common
{
  public interface ICryptoConfigurationProvider
  {
    Type EncryptionAlgorithm { get; }

    byte[] EncryptionKey { get; }

    Type ValidationAlgorithm { get; }

    byte[] ValidationKey { get; }
  }
}