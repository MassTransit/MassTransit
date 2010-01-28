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
namespace Starbucks.Cashier
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Castle.Windsor;
	using log4net.Config;
	using Magnum;
	using Magnum.StateMachine;
	using MassTransit.Saga;
	using MassTransit.Transports.Msmq;
	using MassTransit.WindsorIntegration;
	using Topshelf;
	using Topshelf.Configuration;
	using Topshelf.Configuration.Dsl;

	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			XmlConfigurator.Configure(new FileInfo("cashier.log4net.xml"));

			RunConfiguration cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("StarbucksCashier");
					c.SetDisplayName("Starbucks Cashier");
					c.SetDescription("a Mass Transit sample service for handling orders of coffee.");

					c.RunAsLocalSystem();
					c.DependencyOnMsmq();

					MsmqEndpointConfigurator.Defaults(x => { x.CreateMissingQueues = true; });

					IWindsorContainer container = BootstrapContainer();

					DisplayStateMachine();

					c.ConfigureService<CashierService>(s =>
						{
							s.HowToBuildService(builder => container.Resolve<CashierService>());
							s.WhenStarted(o => o.Start());
							s.WhenStopped(o => o.Stop());
						});
				});
			Runner.Host(cfg, args);
		}

		private static void DisplayStateMachine()
		{
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

			StateMachineInspector.Trace(new CashierSaga(CombGuid.Generate()));
		}

		private static IWindsorContainer BootstrapContainer()
		{
			IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Cashier.Castle.xml");
			container.AddComponent("sagaRepository", typeof (ISagaRepository<>), typeof (InMemorySagaRepository<>));

			container.AddComponent<CashierService>(typeof (CashierService).Name);
			container.AddComponent<CashierSaga>();
			return container;
		}
	}
}