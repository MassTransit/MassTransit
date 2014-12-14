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
namespace MassTransit.NHibernateIntegration.Tests.Sagas
{
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using MassTransit.Tests;
	using TestFramework.Messages;


    [Scenario]
	public class When_configuring_a_service_bus_easily
	{
		[When]
		public void Configuring_a_service_bus_easily()
		{
			FutureMessage<PingMessage> received;
			using (IServiceBus bus = ServiceBusFactory.New(x => { x.ReceiveFrom("loopback://localhost/queue"); }))
			{
				received = new FutureMessage<PingMessage>();

				bus.SubscribeHandler<PingMessage>(received.Set);

				bus.Publish(new PingMessage());
				received.IsAvailable(8.Seconds()).ShouldBeTrue();
			}
		}
	}
}