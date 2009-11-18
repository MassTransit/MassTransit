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
namespace MassTransit.Tests.Distributor
{
	using System;
	using System.Collections.Generic;
	using Configuration;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit.Distributor;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class Default_distributor_specifications :
		LoopbackDistributorTestFixture
	{
		private Dictionary<string, ServiceInstance> _instances;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_instances = new Dictionary<string, ServiceInstance>();

			var instance = new ServiceInstance("loopback://localhost/a", EndpointFactory, SubscriptionServiceUri, x =>
				{
					// setup our worker on the service instance
					x.ImplementDistributorWorker<FirstCommand>(FirstCommandConsumer);
				});

			_instances.Add("A", instance);
		}

		protected override void ConfigureLocalBus(IServiceBusConfigurator configurator)
		{
			configurator.UseDistributorFor<FirstCommand>(EndpointFactory);
		}

		private Action<FirstCommand> FirstCommandConsumer(FirstCommand message)
		{
			return command =>
				{
					var response = new FirstResponse(command.CorrelationId);

					CurrentMessage.Respond(response);
				};
		}

		[Test]
		public void Starting_one_up_should_work()
		{
			ThreadUtil.Sleep(1.Seconds());

			var future = new MassTransit.FutureMessage<FirstResponse>();
			RemoteBus.Subscribe(future);


			ThreadUtil.Sleep(2.Seconds());

			var command = new FirstCommand(CombGuid.Generate());

			LocalBus.Endpoint.Send(command, x => x.SendResponseTo(RemoteBus.Endpoint));

			future.WaitUntilAvailable(5.Seconds()).ShouldBeTrue("No response received");
		}
	}
}