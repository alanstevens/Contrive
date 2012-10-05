using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common.Sql
{
  internal class SqlConnectionStartupTask : IStartupTask
  {
    public void OnStartup()
    {
      ConnectionProvider.GetConnection = () =>
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
                                             typeof (ConnectionProvider).LogException(ex);
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