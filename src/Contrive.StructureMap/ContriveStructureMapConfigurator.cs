using System;
using System.Diagnostics;
using System.Reflection;
using Contrive.Common;
using StructureMap;
using StructureMap.Graph;
using log4net;

namespace Contrive.StructureMap
{
    public static class ContriveStructureMapConfigurator
    {
        static void Interceptors(ConfigurationExpression x)
        {
            x.RegisterInterceptor(new SubscriptionInterceptor());
        }

        static void CustomScanner(IAssemblyScanner s)
        {
            s.Assembly(Assembly.GetAssembly(typeof (IEventAggregator)));
            s.Convention<ListenerConvention>();
            s.AddAllTypesOf<IStartupTask>();
        }

        public static void ConfigureWith(string binDirectory, string rootNamespace)
        {
            try
            {
                StructureMapContainerConfigurator.ConfigureWith(binDirectory, rootNamespace, CustomScanner, Interceptors);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                // a container failure probably means no IoC, so we fall back to basic object usage
                LogManager.GetLogger(typeof (Bootstrapper).Name).Fatal(ex);
            }
        }
    }
}