using System;

namespace Contrive.Common.Data
{
    public static class ConnectionStringProvider
    {
        public static Func<string> GetConnectionString = () => "";
    }
}