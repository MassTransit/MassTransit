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
namespace MassTransit.Tests.Services.LoadBalancer
{
	using Configuration;
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Services.LoadBalancer.Configuration;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestConsumers;
	using TextFixtures;

	[TestFixture]
	public class LoadBalancer_Specs :
		LoopbackTestFixture
	{
		public IServiceBus RemoteBus { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			RemoteBus = ServiceBusConfigurator.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/mt_server");

					x.ConfigureService<LoadBalancerServiceConfigurator>(y => { });
				});

			ObjectBuilder.Stub(x => x.GetInstance<TestMessageConsumer<PingMessage>>())
				.Return(new TestMessageConsumer<PingMessage>());

			LocalBus.Subscribe<TestMessageConsumer<PingMessage>>();
			RemoteBus.Subscribe<TestMessageConsumer<PingMessage>>();
		}

		protected override void TeardownContext()
		{
			RemoteBus.Dispose();
			RemoteBus = null;

			base.TeardownContext();
		}

		[Test]
		public void Example_syntax_for_the_load_balancer()
		{
			var ping = new PingMessage();

			RemoteBus.Execute(ping);

			TestConsumerBase<PingMessage>.AnyShouldHaveReceivedMessage(ping, 5.Seconds());
		}
	}
}