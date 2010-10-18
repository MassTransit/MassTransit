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
namespace MassTransit.RuntimeServices
{
	using System;
	using System.IO;
	using log4net;
	using log4net.Config;
	using Magnum.Reflection;
	using Services.HealthMonitoring;
	using Services.Subscriptions.Server;
	using Services.Timeout;
	using StructureMap;
	using StructureMap.Configuration.DSL;
	using Topshelf;
	using Topshelf.Configuration;
	using Topshelf.Configuration.Dsl;
	using Transports.Msmq;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			BootstrapLogger();

			MsmqEndpointConfigurator.Defaults(x => { x.CreateMissingQueues = true; });

			ObjectFactory.Initialize(x => { x.For<IConfiguration>().Use<Configuration>(); });

			var serviceConfiguration = ObjectFactory.GetInstance<IConfiguration>();

			RunConfiguration configuration = RunnerConfigurator.New(config =>
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

					config.DependencyOnMsmq();
					
					if (serviceConfiguration.SubscriptionServiceEnabled)
					{
						config.ConfigureService<SubscriptionService>(service => { ConfigureService<SubscriptionService, SubscriptionServiceRegistry>(service, start => start.Start(), stop => stop.Stop()); });
					}

					if (serviceConfiguration.HealthServiceEnabled)
					{
						config.ConfigureService<HealthService>(service => { ConfigureService<HealthService, HealthServiceRegistry>(service, start => start.Start(), stop => stop.Stop()); });
					}

					if (serviceConfiguration.TimeoutServiceEnabled)
					{
						config.ConfigureService<TimeoutService>(service => { ConfigureService<TimeoutService, TimeoutServiceRegistry>(service, start => start.Start(), stop => stop.Stop()); });
					}

					config.AfterStoppingTheHost(x => { _log.Info("MassTransit Runtime Services are exiting..."); });
				});
			Runner.Host(configuration, args);
		}

		private static void BootstrapLogger()
		{
			string configFileName = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + typeof (Program).Namespace + ".log4net.xml";

			XmlConfigurator.ConfigureAndWatch(new FileInfo(configFileName));

			_log.Info("Loading " + typeof (Program).Namespace + " Services...");
		}

		private static void ConfigureService<TService, TRegistry>(IServiceConfigurator<TService> service, Action<TService> start, Action<TService> stop)
			where TRegistry : Registry
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

			TRegistry registry = FastActivator<TRegistry>.Create(container);

			container.Configure(x => x.AddRegistry(registry));

			service.HowToBuildService(builder => container.GetInstance<TService>());
			service.WhenStarted(start);
			service.WhenStopped(stop);
		}
	}
}