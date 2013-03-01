using System.Collections.Specialized;
using Contrive.Common;

namespace Contrive.Auth
{
    public interface IAuthConfigurationProvider : IConfigurationProvider
    {
        NameValueCollection UserServiceConfiguration { get; }
    }
}