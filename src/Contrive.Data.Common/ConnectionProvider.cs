using System;
using System.Data;

namespace Contrive.Data.Common
{
  public static class ConnectionProvider
  {
    public static Func<IDbConnection> GetConnection = () => null;
  }
}