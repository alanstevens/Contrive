using System;
using System.Data;
using Contrive.Common.Extensions;

namespace Contrive.Common.Data
{
    public static class AddDbParameterExtensions
    {
        const int VAR_CHAR_MAX = 4000;

        public static IDbCommand AddParameter(this IDbCommand command, string name, DbType type)
        {
            AddParameter(command, name, type, null);
            return command;
        }

        public static IDbCommand AddParameter(this IDbCommand command, string name, DbType type, object value)
        {
            AddParameter(command, name, type, value, 0);
            return command;
        }

        public static IDbCommand AddParameter(this IDbCommand command, string name, DbType type, object value, int size)
        {
            AddParameter(command, name, type, value, size, ParameterDirection.Input);
            return command;
        }

        public static IDbCommand AddOutputParameter(this IDbCommand command, string name, DbType paramType, int size = 0)
        {
            AddParameter(command, name, paramType, null, size, ParameterDirection.Output);
            return command;
        }

        public static IDbCommand AddParameter(this IDbCommand command, string name, DbType type, object value, int size, ParameterDirection direction)
        {
            var parameter = command.NewParameter(name, type, direction, value, size);

            command.Parameters.Add(parameter);
            return command;
        }

        static IDataParameter NewParameter(this IDbCommand command, string name, DbType type, ParameterDirection direction, object value = null, int size = 0, byte precision = (byte) 0, byte scale = (byte) 0)
        {
            if (value.IsNull()) value = DBNull.Value;

            if (type == DbType.AnsiString && size == 0) size = VAR_CHAR_MAX;

            var parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.DbType = type;
            parameter.Direction = direction;
            parameter.Value = value;
            parameter.Size = size;
            parameter.Precision = precision;
            parameter.Scale = scale;

            return parameter;
        }
    }
}