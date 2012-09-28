using System;
using System.Diagnostics;
using System.Reflection;
using Contrive.Common;
using StructureMap;
using StructureMap.Graph;
using log4net;

namespace Contrive.StructureMap
{
  public static class ContriveConfigurator
  {
    public static void Configure(string binDirectory, string rootNamespace)
    {
      Action<ConfigurationExpression> _interceptors = x => x.RegisterInterceptor(new SubscriptionInterceptor());

      Action<IAssemblyScanner> _customScanner = s =>
                                                {
                                                  s.Assembly(Assembly.GetAssembly(typeof (IEventAggregator)));
                                                  s.Convention<ListenerConvention>();
                                                  s.AddAllTypesOf<IStartupTask>();
                                                };

      try
      {
        ContainerConfigurator.ConfigureWith(binDirectory, rootNamespace, _customScanner, _interceptors);
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