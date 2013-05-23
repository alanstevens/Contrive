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
            IDbConnection connection;
            try
            {
                var connectionString = ConnectionStringProvider.GetConnectionString();
                connection = NewConnection(connectionString);
                if(connection.IsNull()) throw new DataException("ConnectionProvider returned a null connection.");
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