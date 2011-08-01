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
namespace MassTransit.Transports.Msmq.Tests.Serialization
{
	using Magnum.Extensions;
	using MassTransit.Serialization;
	using MassTransit.Tests;
	using MassTransit.Tests.Messages;
	using NUnit.Framework;
	using TestFixtures;
	using TestFramework;

	[TestFixture, Explicit]
	public class When_sending_a_message_using_the_specified_serializer<TSerializer> :
		MsmqEndpointTestFixture
		where TSerializer : IMessageSerializer, new()
	{
		public When_sending_a_message_using_the_specified_serializer()
		{
			ConfigureEndpointFactory(x => x.SetDefaultSerializer<TSerializer>());
		}

		[Test]
		public void The_destination_address_should_be_properly_set_on_the_message_envelope()
		{
			var ping = new PingMessage();

			var received = new FutureMessage<PingMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(RemoteBus.Endpoint.Address.Uri, LocalBus.Context().DestinationAddress);

					received.Set(message);
				});

			LocalBus.ShouldHaveSubscriptionFor<PingMessage>();
			LocalBus.Publish(ping);

			Assert.IsTrue(received.IsAvailable(10.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_fault_address_should_be_properly_set_on_the_message_envelope()
		{
			var ping = new PingMessage();

			var received = new FutureMessage<PingMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Address.Uri, LocalBus.Context().FaultAddress);

					received.Set(message);
				});

			LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

			LocalBus.Publish(ping, context => context.SendFaultTo(LocalBus.Endpoint.Address.Uri));

			Assert.IsTrue(received.IsAvailable(10.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_message_type_should_be_properly_set_on_the_message_envelope()
		{
			var ping = new PingMessage();

			var received = new FutureMessage<PingMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(typeof(PingMessage).ToMessageName(), LocalBus.Context().MessageType);

					received.Set(message);
				});

			LocalBus.ShouldHaveSubscriptionFor<PingMessage>();
			LocalBus.Publish(ping);

			Assert.IsTrue(received.IsAvailable(10.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_response_address_should_be_properly_set_on_the_message_envelope()
		{
			var ping = new PingMessage();

			var received = new FutureMessage<PingMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Address.Uri, LocalBus.Context().ResponseAddress);

					received.Set(message);
				});

			LocalBus.ShouldHaveSubscriptionFor<PingMessage>();
			LocalBus.Publish(ping, context => context.SendResponseTo(LocalBus.Endpoint.Address.Uri));

			Assert.IsTrue(received.IsAvailable(10.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_retry_count_should_be_properly_set_on_the_message_envelope()
		{
			var ping = new PingMessage();

			var received = new FutureMessage<PingMessage>();

			int retryCount = 69;
			RemoteBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(retryCount, LocalBus.Context().RetryCount);

					received.Set(message);
				});

			LocalBus.ShouldHaveSubscriptionFor<PingMessage>();
			LocalBus.Publish(ping, context => context.SetRetryCount(retryCount));

			Assert.IsTrue(received.IsAvailable(10.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_source_address_should_be_properly_set_on_the_message_envelope()
		{
			var ping = new PingMessage();

			var received = new FutureMessage<PingMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(message =>
				{
					Assert.AreEqual(LocalBus.Endpoint.Address.Uri, LocalBus.Context().SourceAddress);

					received.Set(message);
				});

			LocalBus.ShouldHaveSubscriptionFor<PingMessage>();
			LocalBus.Publish(ping);

			Assert.IsTrue(received.IsAvailable(10.Seconds()), "Timeout waiting for message");
		}
	}

	[TestFixture, Category("Integration")]
	public class For_the_binary_message_serializer :
		When_sending_a_message_using_the_specified_serializer<BinaryMessageSerializer>
	{
	}

	[TestFixture, Category("Integration")]
	public class For_the_DotNetXml_message_serializer :
		When_sending_a_message_using_the_specified_serializer<DotNotXmlMessageSerializer>
	{
	}

	[TestFixture, Category("Integration")]
	public class For_the_xml_message_serializer :
		When_sending_a_message_using_the_specified_serializer<XmlMessageSerializer>
	{
	}
}