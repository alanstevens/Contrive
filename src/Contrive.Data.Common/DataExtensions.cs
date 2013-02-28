using System;
using System.Data;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
    public static class DataExtensions
    {
        public static IDbCommand GetCommand(this IUnitOfWork unitOfWork)
        {
            return unitOfWork.GetCommandObject().As<IDbCommand>();
        }

        public static bool IsDBNull(this object value)
        {
            return Convert.IsDBNull(value);
        }

        public static bool IsNotDBNull(this object value)
        {
            return !Convert.IsDBNull(value);
        }
    }
}