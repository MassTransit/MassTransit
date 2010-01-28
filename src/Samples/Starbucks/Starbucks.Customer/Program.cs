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
namespace Starbucks.Customer
{
	using System;
	using System.Windows.Forms;
	using Castle.Windsor;
	using MassTransit.Transports.Msmq;
	using MassTransit.WindsorIntegration;

	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			MsmqEndpointConfigurator.Defaults(x => { x.CreateMissingQueues = true; });

			BootstrapContainer();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new OrderDrinkForm());
		}

		private static void BootstrapContainer()
		{
			IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Customer.Castle.xml");
			container.AddComponent<CustomerService>(typeof (CustomerService).Name);
			container.AddComponent<OrderDrinkForm>();
		}
	}
}