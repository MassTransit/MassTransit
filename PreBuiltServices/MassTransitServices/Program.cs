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
namespace MassTransitServices
{
	using System.IO;
	using log4net;
	using log4net.Config;
	using MassTransit.Services.Subscriptions.Server;
	using MassTransit.Services.Timeout;
	using MassTransit.StructureMapIntegration;
	using StructureMap;
	using StructureMap.Attributes;
	using Topshelf;
	using Topshelf.Configuration;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("MassTransitServices.log4net.xml"));
			_log.Info("Loading MassTransit Runtime Services...");

			var configuration = RunnerConfigurator.New(config =>
				{
					config.SetServiceName("MassTransitServices");
					config.SetDisplayName("MassTransit Runtime Services");
					config.SetDescription("Services to support a MassTransit implementation");

					config.RunAsLocalSystem();

					config.DependencyOnMsmq();
					config.DependencyOnMsSql();

					config.BeforeStart(x => { });

					config.ConfigureService<SubscriptionService>(service => ConfigureSubscriptionService(service));

					config.ConfigureService<TimeoutService>(service => ConfigureTimeoutService(service));

					config.AfterStop(x => { _log.Info("MassTransit Runtime Services is exiting..."); });
				});
			Runner.Host(configuration, args);
		}

		private static void ConfigureSubscriptionService(IServiceConfigurator<SubscriptionService> service)
		{
			service.CreateServiceLocator(() =>
				{
					var container = new Container(x =>
						{
							x.ForRequestedType<IConfiguration>()
								.CacheBy(InstanceScope.Singleton)
								.AddConcreteType<Configuration>();
						});

					var registry = new SubscriptionServiceRegistry(container);

					container.Configure(x => x.AddRegistry(registry));

					return new StructureMapObjectBuilder(container);
				});
			service.WhenStarted(instance => instance.Start());
			service.WhenStopped(instance => instance.Stop());
			service.WithName("Subscription Service");
		}

		private static void ConfigureTimeoutService(IServiceConfigurator<TimeoutService> service)
		{
			service.CreateServiceLocator(() =>
				{
					var container = new Container(x =>
						{
							x.ForRequestedType<IConfiguration>()
								.CacheBy(InstanceScope.Singleton)
								.AddConcreteType<Configuration>();
						});

					var registry = new TimeoutServiceRegistry(container);

					container.Configure(x => x.AddRegistry(registry));

					return new StructureMapObjectBuilder(container);
				});
			service.WhenStarted(instance => instance.Start());
			service.WhenStopped(instance => instance.Stop());
			service.WithName("Timeout Service");
		}
	}
}