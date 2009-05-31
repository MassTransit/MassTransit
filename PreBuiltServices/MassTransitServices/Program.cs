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
	using Services.HealthMonitoring;
	using Services.Subscriptions.Server;
	using Services.Timeout;
	using StructureMap;
	using StructureMap.Attributes;
	using StructureMap.Configuration.DSL;
	using StructureMapIntegration;
	using Topshelf;
	using Topshelf.Configuration;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			BootstrapLogger();

			var configuration = RunnerConfigurator.New(config =>
				{
					config.SetServiceName(typeof (Program).Namespace);
					config.SetDisplayName(typeof (Program).Namespace);
					config.SetDescription("MassTransit Runtime Services (Subscriptions, Timeouts, Health Monitoring, Deferred Messages)");

					config.RunAsLocalSystem();

					config.DependencyOnMsmq();
					config.DependencyOnMsSql();

					config.BeforeStart(x => { });

					config.ConfigureService<SubscriptionService>(service =>
						{
							ConfigureService<SubscriptionService, SubscriptionServiceRegistry>(service, start => start.Start(), stop => stop.Stop());
						});

					config.ConfigureService<TimeoutService>(service =>
						{
							ConfigureService<TimeoutService, TimeoutServiceRegistry>(service, start => start.Start(), stop => stop.Stop());
						});

					config.ConfigureService<HealthService>(service =>
						{
							ConfigureService<HealthService, HealthServiceRegistry>(service, start => start.Start(), stop => stop.Stop());
						});

					config.AfterStop(x => { _log.Info("MassTransit Runtime Services are exiting..."); });
				});
			Runner.Host(configuration, args);
		}

		private static void BootstrapLogger()
		{
			var configFileName = typeof (Program).Namespace + ".log4net.xml";

			XmlConfigurator.ConfigureAndWatch(new FileInfo(configFileName));

			_log.Info("Loading " + typeof (Program).Namespace + " Services...");
		}

		private static void ConfigureService<TService, TRegistry>(IServiceConfigurator<TService> service, Action<TService> start, Action<TService> stop)
			where TRegistry : Registry
		{
			service.CreateServiceLocator(() =>
				{
					var container = new Container(x =>
						{
							x.ForRequestedType<IConfiguration>()
								.CacheBy(InstanceScope.Singleton)
								.AddConcreteType<Configuration>();
						});

					TRegistry registry = (TRegistry) Activator.CreateInstance(typeof (TRegistry), container);

					container.Configure(x => x.AddRegistry(registry));

					return new StructureMapObjectBuilder(container);
				});
			service.WhenStarted(start);
			service.WhenStopped(stop);

			service.WithName(typeof (TService).Name);
		}
	}
}