using System;
using System.Data;
using System.Diagnostics;
using Contrive.Common.Extensions;

namespace Contrive.Common.Data
{
    internal class ConnectionProvider
    {
        internal static Func<string, IDbConnection> NewConnection = connectionString => null;

        internal static IDbConnection GetConnection()
        {
            IDbConnection connection;
            try
            {
                var connectionString = ConnectionStringProvider.GetConnectionString();
                connection = NewConnection(connectionString);
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
        }
    }
}