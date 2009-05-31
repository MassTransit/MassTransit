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
	using System.IO;
	using log4net;
	using log4net.Config;
	using Services.Subscriptions.Server;
	using Services.Timeout;
	using StructureMap;
	using StructureMap.Attributes;
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
					config.SetServiceName(typeof(Program).Namespace);
					config.SetDisplayName(typeof(Program).Namespace);
					config.SetDescription("MassTransit Runtime Services (Subscriptions, Timeouts, Health Monitoring, Deferred Messages)");

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

		private static void BootstrapLogger()
		{
			var configFileName = typeof(Program).Namespace + ".log4net.xml";

			XmlConfigurator.ConfigureAndWatch(new FileInfo(configFileName));

			_log.Info("Loading " + typeof(Program).Namespace + " Services...");
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