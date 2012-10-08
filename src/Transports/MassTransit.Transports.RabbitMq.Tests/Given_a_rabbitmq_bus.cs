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
namespace MassTransit.Transports.RabbitMq.Tests
{
	using System;
	using BusConfigurators;
	using Magnum.TestFramework;
	using TestFramework.Fixtures;

	[Scenario]
	public abstract class Given_a_rabbitmq_bus :
		LocalTestFixture<RabbitMqTransportFactory>
	{
		protected Given_a_rabbitmq_bus()
		{
			LocalUri = new Uri("rabbitmq://localhost:5672/test_queue");
			LocalErrorUri = new Uri("rabbitmq://localhost:5672/test_queue_error");

			ConfigureEndpointFactory(x => { x.UseRabbitMq(); });
		}

		protected Uri LocalErrorUri { get; private set; }

		protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
		{
			base.ConfigureServiceBus(uri, configurator);

			configurator.UseRabbitMq();
		}
	}
}