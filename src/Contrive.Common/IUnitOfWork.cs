using System;

namespace Contrive.Common
{
    public interface IUnitOfWork : IDisposable
    {
        object GetCommandObject();
    }
}