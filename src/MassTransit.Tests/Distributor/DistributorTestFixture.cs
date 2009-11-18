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
	using TextFixtures;

	public class DistributorTestFixture<TEndpoint> :
		SubscriptionServiceTestFixture<TEndpoint>
		where TEndpoint : IEndpoint
	{
		protected override void TeardownContext()
		{
			Instances.Each(x => x.Value.Dispose());

			Instances.Clear();

			base.TeardownContext();
		}

		protected override void EstablishContext()
		{
			base.EstablishContext();

			Instances = new Dictionary<string, ServiceInstance>();
		}

		protected override void ConfigureLocalBus(IServiceBusConfigurator configurator)
		{
			configurator.UseDistributorFor<FirstCommand>(EndpointFactory);
		}

		protected Dictionary<string, ServiceInstance> Instances { get; private set; }

		protected void AddInstance(string instanceName, string queueName)
		{
			var instance = new ServiceInstance(queueName, EndpointFactory, SubscriptionServiceUri, x =>
				{
					// setup our worker on the service instance
					x.ImplementDistributorWorker<FirstCommand>(FirstCommandConsumer);
				});

			Instances.Add(instanceName, instance);
		}

		private static Action<FirstCommand> FirstCommandConsumer(FirstCommand message)
		{
			return command =>
				{
					ThreadUtil.Sleep(10.Milliseconds());

					var response = new FirstResponse(command.CorrelationId);

					CurrentMessage.Respond(response);
				};
		}
	}
}