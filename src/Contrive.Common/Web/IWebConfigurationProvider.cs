using System.Collections.Generic;

namespace Contrive.Common.Web
{
  public interface IWebConfigurationProvider: IConfigurationProvider
  {
    string DefaultRedirect { get; }

    Dictionary<int, string> CustomRedirects { get; }

    string CustomErrorsMode { get; }
  }
}
