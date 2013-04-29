using System;
using System.Data;

namespace Contrive.Common.Data
{
    public static class CommandProvider
    {
        public static Func<IDbCommand> GetCommand = () =>
                                                    {
                                                        var connection = ConnectionProvider.GetConnection();
                                                        var command = connection.CreateCommand();
                                                        return command;
                                                    };
    }
}