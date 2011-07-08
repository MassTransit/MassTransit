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
namespace MassTransit.Tests.Services.Routing
{
	using System;
	using MassTransit.Services.Routing.Configuration;
	using Messages;
	using NUnit.Framework;
	using TextFixtures;
	using TestFramework;

	[TestFixture]
	public class When_configuring_routes :
		LoopbackTestFixture
	{
		[Test]
		public void Should_create_outbound_sink_for_route()
		{
			Uri address = new Uri("loopback://localhost/test_target");

			var configurator = new RoutingConfigurator();

			configurator.Route<PingMessage>().To(address);
			configurator.Route<PongMessage>().To(address);

			IBusService busService = configurator.Create(LocalBus);
			busService.Start(LocalBus);

			LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
			LocalBus.ShouldHaveRemoteSubscriptionFor<PongMessage>();

			busService.Stop();

			LocalBus.ShouldNotHaveSubscriptionFor<PingMessage>();

			busService.Dispose();
		}
	}
}