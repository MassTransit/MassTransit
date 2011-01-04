// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Transports.Msmq.Tests
{
	using System;
	using System.IO;
	using System.Messaging;
	using Internal;
	using Magnum;
	using MassTransit.Serialization;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture, Category("Integration")]
	public class Calling_receive_on_the_endpoint
	{
		[Test,Ignore("Not testing as I think it was about the old lambda stuff")]
		public void Should_invoke_the_continuation()
		{
			var transport = MockRepository.GenerateStub<ITransport>();
			transport.Stub(x => x.Receive(null))
				.Callback(new Func<Func<IReceivingContext, Action<IReceivingContext>>, bool>(Forwarder));

			var address = MockRepository.GenerateMock<IEndpointAddress>();

			IEndpoint endpoint = new Endpoint(address, new XmlMessageSerializer(), transport, MockRepository.GenerateMock<ITransport>());

			var future = new Future<object>();

			endpoint.Receive(m =>
			                 message =>
			                 	{
			                 		Assert.IsInstanceOf<SimpleMessage>(message);

			                 		Assert.AreEqual(((SimpleMessage) message).Name, "Chris");

			                 		future.Complete(message);
			                 	});

            Assert.IsTrue(future.IsCompleted, "Receive was not called");
		}

		private bool Forwarder(Func<IReceivingContext, Action<IReceivingContext>> arg)
		{
			using (Message message = CreateSimpleMessage())
			{
				Action<IReceivingContext> func = arg(new MsmqReceivingContext(message));
				if (func == null)
					return true;

				func(new MsmqReceivingContext(message));
			}

			return true;
		}

		private Message CreateSimpleMessage()
		{
			var message = new Message();
			new XmlMessageSerializer().Serialize(message.BodyStream, new SimpleMessage {Name = "Chris"});
			message.BodyStream.Seek(0, SeekOrigin.Begin);

			return message;
		}
	}

	public class SimpleMessage
	{
		public string Name { get; set; }
	}
}