using System;
using System.Reflection;
using Contrive.Common;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using StructureMap.Graph;
using StructureMap.ServiceLocatorAdapter;

namespace Contrive.StructureMap
{
    public static class StructureMapContainerConfigurator
    {
        static Action<IAssemblyScanner> CustomScanner = s =>
                                                        {
                                                            s.Assembly(Assembly.GetAssembly(typeof (IEventAggregator)));
                                                            s.Convention<ListenerConvention>();
                                                            s.AddAllTypesOf<IStartupTask>();
                                                        };

        static Action<ConfigurationExpression> Interceptors = x => x.RegisterInterceptor(new SubscriptionInterceptor());

        static bool _configured;

        public static void ConfigureWith(string assemblyDirectory, string rootNamespace)
        {
            if (_configured) return;

            if (CustomScanner.IsNull()) CustomScanner = s => { };

            if (Interceptors.IsNull()) Interceptors = x => { };

            var container = new Container();

            var serviceLocator = new StructureMapServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            container.Configure(x =>
                                {
                                    x.For<IContainer>().Use(container);
                                    x.For<IServiceLocator>().Singleton().Use(serviceLocator);
                                    Interceptors(x);
                                    x.Scan(s =>
                                           {
                                               s.Assembly(Assembly.GetEntryAssembly());
                                               s.Assembly(Assembly.GetExecutingAssembly());
                                               if (assemblyDirectory.IsNotBlank() && rootNamespace.IsNotBlank())
                                               {
                                                   s.AssembliesFromPath(assemblyDirectory,
                                                                        assembly =>
                                                                        assembly.GetName().Name.StartsWith(rootNamespace));
                                               }
                                               s.WithDefaultConventions();
                                               s.LookForRegistries();
                                               CustomScanner(s);
                                           });
                                });

            _configured = true;
        }
    }
}