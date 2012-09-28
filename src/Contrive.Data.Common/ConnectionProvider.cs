using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
  public class ConnectionProvider : IStartupTask
  {
    public static Func<IDbConnection> GetConnection = () => null;

    public void OnStartup()
    {
      GetConnection = () =>
                      {
                        SqlConnection connection = null;
                        try
                        {
                          var connectionString = ConnectionStringProvider.GetConnectionString();
                          connection = new SqlConnection(connectionString);
                          connection.Open();
                        }
                        catch (Exception ex)
                        {
                          typeof(ConnectionProvider).LogException(ex);
#if DEBUG
                          Debugger.Break();
#endif
                          throw;
                        }
                        return connection;
                      };
    }
  }
}