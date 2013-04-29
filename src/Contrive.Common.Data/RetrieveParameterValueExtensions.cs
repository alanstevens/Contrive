using System;
using System.Data;
using Contrive.Common.Extensions;

namespace Contrive.Common.Data
{
    public static class RetrieveParameterValueExtensions
    {
        public static int GetInt(this IDbCommand command, string name)
        {
            return command.GetValue<int>(name);
        }

        public static bool GetBool(this IDbCommand command, string name)
        {
            return command.GetValue<bool>(name);
        }

        public static string GetString(this IDbCommand command, string name)
        {
            return command.GetValue<string>(name);
        }

        public static Guid GetGuid(this IDbCommand command, string name)
        {
            return command.GetValue<Guid>(name);
        }

        public static char GetChar(this IDbCommand command, string name)
        {
            return command.GetValue<char>(name);
        }

        public static DateTime GetDate(this IDbCommand command, string name)
        {
            return command.GetValue<DateTime>(name);
        }

        public static decimal GetDecimal(this IDbCommand command, string name)
        {
            return command.GetValue<Decimal>(name);
        }

        public static T GetValue<T>(this IDbCommand command, string name)
        {
            var parameter = command.Parameters[name].As<IDbDataParameter>();
            return parameter.GetValue<T>();
        }

        public static T GetValue<T>(this IDataParameter parameter)
        {
            var value = parameter.Value;

            return value.IsNotDBNull() ? value.As<T>() : default(T);
        }
    }
}