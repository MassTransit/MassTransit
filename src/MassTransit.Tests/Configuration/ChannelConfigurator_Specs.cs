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
namespace MassTransit.Tests.Configuration
{
	using System;
	using Magnum.TestFramework;
	using MassTransit.Pipeline.Sinks;
	using SubscriptionConnectors;

	[Scenario]
	public class When_configuring_a_channel_using_the_connector
	{
		[When]
		public void Configuring_a_channel_using_the_connector()
		{
			var bus = ServiceBusFactory.New(x => x.ReceiveFrom("loopback://localhost/my_queue"));

//			bus.Configure(configurator =>
//				{
//				});
		}

		[Then]
		public void Should_include_the_proper_nodes()
		{
		}

		class A
		{
		}

		class Consumer :
			Consumes<A>.All
		{
			public void Consume(A message)
			{
			}
		}
	}
}