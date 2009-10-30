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
namespace GatewayService
{
	using System;
	using System.IO;
	using log4net;
	using log4net.Config;
	using MassTransit.StructureMapIntegration;
	using MassTransit.Transports.Msmq;
	using StructureMap;
	using Topshelf;
	using Topshelf.Configuration;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			BootstrapLogger();

			MsmqEndpointConfigurator.Defaults(x => { x.CreateMissingQueues = true; });

			var configuration = RunnerConfigurator.New(config =>
				{
					config.SetServiceName(typeof (Program).Namespace);
					config.SetDisplayName(typeof (Program).Namespace);
					config.SetDescription(typeof (Program).Namespace);

					config.RunAsLocalSystem();

					config.DependencyOnMsmq();
					config.DependencyOnMsSql();

					ObjectFactory.Configure(x =>
						{
							x.AddRegistry(new GatewayServiceRegistry());
						});

					config.ConfigureService<OrderServiceGateway>(typeof (OrderServiceGateway).Name, service =>
						{
							service.CreateServiceLocator(() => new StructureMapObjectBuilder(ObjectFactory.Container));
							service.WhenStarted(x => x.Start());
							service.WhenStopped(x => x.Stop());
						});


					config.AfterStoppingTheHost(x => { _log.Info("Exiting..."); });
				});
			Runner.Host(configuration, args);
		}

		private static void BootstrapLogger()
		{
			var name = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, typeof (Program).Namespace + ".log4net.xml");

			XmlConfigurator.ConfigureAndWatch(new FileInfo(name));

			_log.Info("Loading " + typeof (Program).Namespace + " Services...");
		}
	}
}