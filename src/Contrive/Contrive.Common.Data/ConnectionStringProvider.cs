using System;

namespace Contrive.Common.Data
{
    internal static class ConnectionStringProvider
    {
        public static Func<string> GetConnectionString = () => "";
    }
}