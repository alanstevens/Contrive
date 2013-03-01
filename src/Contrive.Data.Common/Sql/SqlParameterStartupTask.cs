using System.Data.SqlClient;
using Contrive.Common;

namespace Contrive.Data.Common.Sql
{
    /// <summary>
    ///     This task sets the default IDataParameter to be a SqlParameter
    /// </summary>
    public class SqlParameterStartupTask : IStartupTask
    {
        public void OnStartup()
        {
            AddDbParameterExtensions.CreateParameter =
                (name, type, direction, value, size, precision, scale) =>
                new SqlParameter
                {
                    ParameterName = name,
                    DbType = type,
                    Direction = direction,
                    Value = value,
                    Size = size,
                    Precision = precision,
                    Scale = scale
                };
        }
    }
}