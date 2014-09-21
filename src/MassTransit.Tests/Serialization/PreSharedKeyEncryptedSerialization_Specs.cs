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
namespace MassTransit.Tests.Serialization
{
	using System;
	using System.IO;
	using Context;
	using Magnum.TestFramework;
	using MassTransit.Serialization;
	using Messages;
	using NUnit.Framework;

	public abstract class PreSharedKeyEncryptedSerialization_Specs<TSerializer>
		where TSerializer : IMessageSerializer, new()
	{
		PartialSerializationTestMessage _message;

		string _key = "eguhidbehumjdemy1234567890123456";

		[SetUp]
		public void SetupContext()
		{
			_message = new PartialSerializationTestMessage
				{
					DecimalValue = 123.45m,
					LongValue = 098123213,
					BoolValue = true,
					ByteValue = 127,
					IntValue = 123,
					DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
					GuidValue = new Guid("B00C3BD0-3CE9-4B14-9EC6-E7348084EF1F"),
					StringValue = "Chris's Sample Code",
					DoubleValue = 1823.172,
				};
		}

		[Test]
		public void The_encrypted_serializer_should_be_awesome()
		{
			byte[] serializedMessageData;

			var serializer = new PreSharedKeyEncryptedMessageSerializer(_key, new TSerializer());

			using (var output = new MemoryStream())
			{
				serializer.Serialize(output, _message.ToSendContext());

				serializedMessageData = output.ToArray();

				//          Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
			}

			using (var input = new MemoryStream(serializedMessageData))
			{
				ReceiveContext receiveContext = ReceiveContext.FromBodyStream(input);
				serializer.Deserialize(receiveContext);

				IConsumeContext<PartialSerializationTestMessage> context;
				receiveContext.TryGetContext(out context).ShouldBeTrue();

				context.ShouldNotBeNull();

				context.Message.ShouldEqual(_message);
			}
		}

		[Test(Description = "Validates that content type is set correctly set so the right serializer is selected at reception.")]
		public void Endpoint_receive_should_select_encrypted_serializer_based_on_content_type()
		{
			var serializer = new PreSharedKeyEncryptedMessageSerializer(_key, new TSerializer());
			var pingMessage = new PingMessage(Guid.NewGuid());
			const string self = "loopback://127.0.0.1/self";
            
			using (var bus = ServiceBusFactory.New(sbc =>
			{
				sbc.ReceiveFrom(self);
				// Ensure outbound message are serialized and encrypted
				sbc.SetDefaultSerializer(serializer);
				sbc.Subscribe(s => s.Handler<PingMessage>((context, message) => context.Respond(new PongMessage(message.CorrelationId))));
			}))
			{
				var selfEndpoint = bus.GetEndpoint(new Uri(self));
				selfEndpoint.ShouldNotBeNull();

				try
				{
					selfEndpoint.SendRequest(pingMessage, bus, x =>
					{
						x.SetTimeout(TimeSpan.FromSeconds(5));
						x.Handle<PongMessage>(pongMsg => pongMsg.CorrelationId.ShouldEqual(pingMessage.CorrelationId));
					});
				}
				catch (Exception e)
				{
					Assert.Fail(e.ToString());
				}
			}
		}
	}

	[TestFixture]
	public class WhenUsingCustomXmlWithEncryption :
		PreSharedKeyEncryptedSerialization_Specs<XmlMessageSerializer>
	{
	}

	[TestFixture]
	public class WhenUsingBinaryWithEncryption :
		PreSharedKeyEncryptedSerialization_Specs<BinaryMessageSerializer>
	{
	}

	[TestFixture]
	public class WhenUsingJsonWithEncryption :
		PreSharedKeyEncryptedSerialization_Specs<JsonMessageSerializer>
	{
	}

//	[TestFixture]
//	public class WhenUsingBsonWithEncryption :
//		PreSharedKeyEncryptedSerialization_Specs<BsonMessageSerializer>
//	{
//	}
}