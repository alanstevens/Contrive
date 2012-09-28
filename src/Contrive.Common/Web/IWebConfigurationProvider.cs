using System.Collections.Generic;
using System.Collections.Specialized;

namespace Contrive.Common.Web
{
  public interface IWebConfigurationProvider: IConfigurationProvider
  {
    NameValueCollection RoleManagerSettings { get; }

    NameValueCollection MembershipSettings { get; }

    string MachineKey { get; }

    string DefaultRedirect { get; }

    Dictionary<int, string> CustomRedirects { get; }

    string CustomErrorsMode { get; }

    string DecryptionAlgorithm { get; }
  }
}
