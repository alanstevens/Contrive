using System.Data.SqlClient;

namespace Contrive.Common.Data
{
    public class ConnectionProviderStartupTask : IStartupTask
    {
        public void OnStartup()
        {
            ConnectionProvider.NewConnection = connectionString => new SqlConnection(connectionString);
        }
    }
}