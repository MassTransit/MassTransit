/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.NMS.Tests
{
	using System;
	using Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_sending_a_message_to_the_queue
	{
		[Test]
		public void The_message_should_arrive()
		{
			NmsEndpoint endpoint = new NmsEndpoint("activemq://localhost:61616/queue_name");

			SimpleMessage msg = new SimpleMessage();
			msg.Name = "Chris";

			endpoint.Send(msg);
		}

		[Test]
		public void The_message_should_be_retrieved()
		{
			using (NmsEndpoint endpoint = new NmsEndpoint("activemq://localhost:61616/queue_name"))
			{
				SimpleMessage msg = new SimpleMessage();
				msg.Name = "Chris";

				endpoint.Send(msg);

				object obj = endpoint.Receive(TimeSpan.FromSeconds(5));

				Assert.That(obj, Is.Not.Null);

				Assert.That(obj, Is.TypeOf(typeof (SimpleMessage)));
			}
		}
	}

	public delegate void EnvelopeHandler(IEnvelope e);

	[Serializable]
	public class SimpleMessage
	{
		private string _name;


		public SimpleMessage()
		{
		}

		public SimpleMessage(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}