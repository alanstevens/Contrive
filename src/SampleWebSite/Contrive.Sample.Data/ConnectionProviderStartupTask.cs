using System.Data.SqlClient;
using Contrive.Common;
using Contrive.Common.Data;

namespace Contrive.Sample.Data
{
    public class ConnectionProviderStartupTask : IStartupTask
    {
        public void OnStartup()
        {
            ConnectionProvider.NewConnection = connectionString => new SqlConnection(connectionString);
        }
    }
}