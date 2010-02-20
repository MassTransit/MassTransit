// Copyright 2007-2008 The Apache Software Foundation.
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
namespace LegacyRuntime
{
    using System;
    using System.IO;
    using log4net;
    using log4net.Config;
    using Magnum.Reflection;
    using MassTransit.LegacySupport;
    using MassTransit.StructureMapIntegration;
    using MassTransit.Transports.Msmq;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using Topshelf;
    using Topshelf.Configuration;
	using Topshelf.Configuration.Dsl;

    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            BootstrapLogger();

            MsmqEndpointConfigurator.Defaults(x =>
            {
                x.CreateMissingQueues = true;
            });

            var cfg = RunnerConfigurator.New(config =>
            {
                config.SetServiceName(typeof(Program).Namespace);
                config.SetDisplayName(typeof(Program).Namespace);
                config.SetDescription("MassTransit Legacy Services");

                config.RunAsLocalSystem();

                config.DependencyOnMsmq();
                config.DependencyOnMsSql();

                config.ConfigureService<LegacySubscriptionProxyService>(s =>
                {
                    ConfigureService<LegacySubscriptionProxyService, LegacySupportRegistry>(s, start=>start.Start(), stop=>stop.Stop());
                });

                config.AfterStoppingTheHost(x => { _log.Info("MassTransit Legacy Services are exiting..."); });
            });

            Runner.Host(cfg, args);
        }
        private static void BootstrapLogger()
        {
            var configFileName = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + typeof(Program).Namespace + ".log4net.xml";

            XmlConfigurator.ConfigureAndWatch(new FileInfo(configFileName));

            _log.Info("Loading " + typeof(Program).Namespace + " Services...");
        }
        private static void ConfigureService<TService, TRegistry>(IServiceConfigurator<TService> service, Action<TService> start, Action<TService> stop)
    where TRegistry : Registry
        {
			var registry = FastActivator<TRegistry>.Create(ObjectFactory.Container);
			ObjectFactory.Configure(cfg =>
			{
				cfg.For<IConfiguration>().Singleton().Use<Configuration>(); 
				cfg.AddRegistry(registry);
			});
			service.HowToBuildService(builder => ObjectFactory.GetInstance<TService>());

            service.WhenStarted(start);
            service.WhenStopped(stop);
        }
    }
}
