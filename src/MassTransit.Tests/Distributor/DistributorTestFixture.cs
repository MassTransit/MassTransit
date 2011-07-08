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
namespace MassTransit.Tests.Distributor
{
	using System;
	using BusConfigurators;
	using Load.Messages;
	using Magnum;
	using Magnum.Extensions;
	using MassTransit.Distributor;
	using MassTransit.Transports;
	using TextFixtures;

	public class DistributorTestFixture<TTransportFactory> :
		SubscriptionServiceTestFixture<TTransportFactory>
		where TTransportFactory : ITransportFactory, new()
	{
		protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
		{
			configurator.UseDistributorFor<FirstCommand>();
		}

		protected void AddFirstCommandInstance(string instanceName, string queueName)
		{
			AddInstance(instanceName, queueName, x =>
				{
					// setup our worker on the service instance
					x.ImplementDistributorWorker<FirstCommand>(FirstCommandConsumer);
				});
		}

		Action<FirstCommand> FirstCommandConsumer(FirstCommand message)
		{
			return command =>
				{
					ThreadUtil.Sleep(10.Milliseconds());

					var response = new FirstResponse(command.CorrelationId);

					LocalBus.Context().Respond(response);
				};
		}
	}
}