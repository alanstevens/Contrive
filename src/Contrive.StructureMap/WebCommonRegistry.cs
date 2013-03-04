using System.Web;
using Contrive.Common;
using Contrive.Web.Common;
using Contrive.Web.Common.Errors;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class WebCommonRegistry : Registry
    {
        public WebCommonRegistry()
        {
            For<IWebConfigurationProvider>().Singleton().Use<WebConfigurationProvider>();
            For<ICache>().HttpContextScoped().Use<Cache>();
            For<IErrorViewRenderer>().Singleton().Use<MvcErrorViewRenderer>();
            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
        }
    }
}