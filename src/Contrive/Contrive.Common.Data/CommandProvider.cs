using System;
using System.Data;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Common.Data
{
    public static class CommandProvider
    {
        /// <summary>
        /// The IDbCommand lifecycle is managed by the inversion of control container.
        /// You *must* add IDbCommand to your container configuration.
        /// A typical configuration would scope the IDbCommand lifecycle to thread or HttpRequest.
        /// </summary>
        public static Func<IDbCommand> GetCommand = () =>
                                                    {
                                                        var connection = ServiceLocator.Current.GetInstance<IDbConnection>();
                                                        var command = connection.CreateCommand();
                                                        return command;
                                                    };
    }
}