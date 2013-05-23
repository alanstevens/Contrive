using System;
using System.Data;
using System.Diagnostics;
using Contrive.Common.Extensions;

namespace Contrive.Common.Data
{
    public class ConnectionProvider
    {
        /// <summary>
        /// This delegate must be populated at application startup using an IStartupTask or some other means
        /// This abstraction enables the client app to use any ADO.NET data provider: http://msdn.microsoft.com/en-us/data/dd363565.aspx
        /// </summary>
        public static Func<string, IDbConnection> NewConnection = connectionString => null;

        public static IDbConnection GetConnection()
        {
            try
            {
                var connectionString = ConnectionStringProvider.GetConnectionString();
                var connection = NewConnection(connectionString);
                if(connection.IsNull()) throw new DataException("ConnectionProvider.NewConnection() returned a null connection.{0}Ensure that NewConnection is populated at application startup".FormatWith(Environment.NewLine));
                connection.Open();
            return connection;
            }
            catch (Exception ex)
            {
                typeof (ConnectionProvider).LogException(ex);
#if DEBUG
                Debugger.Break();
#endif
                throw;
            }
        }
    }
}