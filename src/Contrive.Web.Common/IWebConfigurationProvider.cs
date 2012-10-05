using System.Collections.Generic;
using Contrive.Common;

namespace Contrive.Web.Common
{
  public interface IWebConfigurationProvider : IConfigurationProvider
  {
    string DefaultRedirect { get; }

    Dictionary<int, string> CustomRedirects { get; }

    string CustomErrorsMode { get; }
  }
}