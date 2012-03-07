// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RuntimeServices
{
    using System;
    using System.IO;
    using Log4NetIntegration.Logging;
    using Logging;
    using log4net.Config;
    using Services.HealthMonitoring;
    using Services.Subscriptions.Server;
    using Services.Timeout;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using Topshelf;
    using Topshelf.HostConfigurators;

    class Program
    {
        static readonly ILog _log = Logger.Get(typeof (Program));

        static void Main()
        {
            BootstrapLogger();

            ObjectFactory.Initialize(x => x.For<IConfiguration>().Use<Configuration>());

            var serviceConfiguration = ObjectFactory.GetInstance<IConfiguration>();

            HostFactory.Run(config =>
                {
                    config.SetServiceName(typeof (Program).Namespace);
                    config.SetDisplayName(typeof (Program).Namespace);
                    config.SetDescription("MassTransit Runtime Services (Subscription, Timeout, Health Monitoring)");

                    if (serviceConfiguration.UseServiceCredentials)
                    {
                        config.RunAs(serviceConfiguration.ServiceUsername, serviceConfiguration.ServicePassword);
                    }
                    else
                        config.RunAsLocalSystem();

                    config.DependsOnMsmq();

                    if (serviceConfiguration.SubscriptionServiceEnabled)
                    {
                        config.ConfigureService<SubscriptionService, SubscriptionServiceRegistry>(
                            c => new SubscriptionServiceRegistry(c), start => start.Start(), stop => stop.Stop());
                    }

                    if (serviceConfiguration.HealthServiceEnabled)
                    {
                        config.ConfigureService<HealthService, HealthServiceRegistry>(
                            c => new HealthServiceRegistry(c), start => start.Start(), stop => stop.Stop());
                    }

                    if (serviceConfiguration.TimeoutServiceEnabled)
                    {
                        config.ConfigureService<TimeoutService, TimeoutServiceRegistry>(
                            c => new TimeoutServiceRegistry(c), start => start.Start(), stop => stop.Stop());
                    }

                    config.AfterStoppingServices(x => { _log.Info("MassTransit Runtime Services are exiting..."); });
                });
        }

        static void BootstrapLogger()
        {
            Log4NetLogger.Use(typeof (Program).Namespace + ".log4net.xml");
            Topshelf.Logging.Log4NetLogger.Use();

            _log.Info("Loading " + typeof (Program).Namespace + " Services...");
        }
    }

    public static class ConfigureServiceExtension
    {
        public static void ConfigureService<TService, TRegistry>(this HostConfigurator configurator,
                                                                 Func<IContainer, TRegistry> registry,
                                                                 Action<TService> start, Action<TService> stop)
            where TRegistry : Registry
            where TService : class
        {
            var container = new Container(x =>
                {
                    x.For<IConfiguration>()
                        .Singleton()
                        .Add<Configuration>();

                    x.For<TService>()
                        .Singleton()
                        .Use<TService>();
                });

            container.Configure(x => x.AddRegistry(registry(container)));

            configurator.Service<TService>(service =>
                {
                    service.ConstructUsing(builder => container.GetInstance<TService>());
                    service.WhenStarted(start);
                    service.WhenStopped(stop);
                });
        }
    }
}