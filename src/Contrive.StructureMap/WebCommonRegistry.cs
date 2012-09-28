using Contrive.Common.Web;
using Contrive.Web.Common;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
  public class WebCommonRegistry : Registry
  {
    public WebCommonRegistry()
    {
      For<IWebConfigurationProvider>().Singleton().Use<WebConfigurationProvider>();
      For<ICache>().HttpContextScoped().Use<Cache>();
    }
  }
}