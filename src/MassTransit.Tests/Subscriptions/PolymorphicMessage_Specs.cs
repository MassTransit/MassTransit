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
namespace MassTransit.Tests.Subscriptions
{
	using BusConfigurators;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class When_a_message_is_received_that_is_polymorphic :
		LoopbackLocalAndRemoteTestFixture
	{
		Future<BaseMessage> _baseMessage;
		Future<Message> _message;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			LocalBus.ShouldHaveSubscriptionFor<Message>();
			LocalBus.ShouldHaveSubscriptionFor<BaseMessage>();

			LocalBus.Publish(new Message());
		}

		protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
		{
			base.ConfigureRemoteBus(configurator);

			_baseMessage = new Future<BaseMessage>();
			_message = new Future<Message>();

			configurator.Subscribe(cf =>
				{
					cf.Handler<BaseMessage>(message => _baseMessage.Complete(message));
					cf.Handler<Message>(message => _message.Complete(message));
				});
		}


		public class BaseMessage
		{
		}

		public class Message : 
			BaseMessage
		{
		}

		[Test]
		public void Should_be_received_as_base_message()
		{
			_baseMessage.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
		}

		[Test]
		public void Should_be_received_as_message()
		{
			_message.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
		}
	}
}