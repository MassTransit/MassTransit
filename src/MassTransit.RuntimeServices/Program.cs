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
	using log4net;
	using log4net.Config;
	using Magnum.Reflection;
	using Services.HealthMonitoring;
	using Services.Subscriptions.Server;
	using Services.Timeout;
	using StructureMap;
	using StructureMap.Configuration.DSL;
	using Topshelf;
	using Topshelf.ServiceConfigurators;

	class Program
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (Program));

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
						config.Service<SubscriptionService>(service => ConfigureService<SubscriptionService, SubscriptionServiceRegistry>(service, start => start.Start(), stop => stop.Stop()));
					}

					if (serviceConfiguration.HealthServiceEnabled)
					{
						config.Service<HealthService>(service => ConfigureService<HealthService, HealthServiceRegistry>(service, start => start.Start(), stop => stop.Stop()));
					}

					if (serviceConfiguration.TimeoutServiceEnabled)
					{
						config.Service<TimeoutService>(service => ConfigureService<TimeoutService, TimeoutServiceRegistry>(service, start => start.Start(), stop => stop.Stop()));
					}

					config.AfterStoppingServices(x => { _log.Info("MassTransit Runtime Services are exiting..."); });
				});
		}

		static void BootstrapLogger()
		{
			string configFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, typeof (Program).Namespace + ".log4net.xml");

			XmlConfigurator.ConfigureAndWatch(new FileInfo(configFileName));

			_log.Info("Loading " + typeof (Program).Namespace + " Services...");
		}

		static void ConfigureService<TService, TRegistry>(ServiceConfigurator<TService> service, Action<TService> start, Action<TService> stop)
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

			TRegistry registry = FastActivator<TRegistry>.Create(container);

			container.Configure(x => x.AddRegistry(registry));

			service.ConstructUsing(builder => container.GetInstance<TService>());
			service.WhenStarted(start);
			service.WhenStopped(stop);
		}
	}
}