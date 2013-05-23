using System;

namespace Contrive.Common.Data
{
    public static class ConnectionStringProvider
    {
    /// <summary>
    /// This delegate can be populated at application startup using an IStartupTask or similar means.
    /// This abstraction enables the consumer to choose the default application connection string.
    /// </summary>
        public static Func<string> GetConnectionString = () => new ConfigurationProvider().DefaultConnectionString;
    }
}