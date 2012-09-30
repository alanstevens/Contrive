using System;

namespace Contrive.Data.Common
{
  public static class ConnectionStringProvider
  {
    public static Func<string> GetConnectionString = () => "";
  }
}