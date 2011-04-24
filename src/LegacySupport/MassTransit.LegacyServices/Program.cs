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
namespace LegacyRuntime
{
	using System;
	using System.IO;
	using log4net;
	using log4net.Config;
	using MassTransit.LegacySupport;
	using StructureMap;
	using Topshelf;

	class Program
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		static void Main()
		{
			BootstrapLogger();

			ObjectFactory.Initialize(x => { x.For<IConfiguration>().Use<Configuration>(); });

			HostFactory.Run(config =>
				{
					config.SetServiceName(typeof (Program).Namespace);
					config.SetDisplayName(typeof (Program).Namespace);
					config.SetDescription("MassTransit Legacy Services");

					config.RunAsLocalSystem();

					config.DependsOnMsmq();
					config.DependsOnMsSql();

					config.Service<LegacySubscriptionProxyService>(s =>
						{
							var registry = new LegacySupportRegistry(ObjectFactory.Container);
							ObjectFactory.Configure(cfg =>
								{
									cfg.For<IConfiguration>().Singleton().Use<Configuration>();
									cfg.AddRegistry(registry);
								});

							s.ConstructUsing(builder => ObjectFactory.GetInstance<LegacySubscriptionProxyService>());
							s.WhenStarted(start => start.Start());
							s.WhenStopped(stop => stop.Stop());
						});

					config.AfterStoppingServices(x => { _log.Info("MassTransit Legacy Services are exiting..."); });
				});
		}

		static void BootstrapLogger()
		{
			string configFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, typeof (Program).Namespace + ".log4net.xml");

			XmlConfigurator.ConfigureAndWatch(new FileInfo(configFileName));

			_log.Info("Loading " + typeof (Program).Namespace + " Services...");
		}
	}
}