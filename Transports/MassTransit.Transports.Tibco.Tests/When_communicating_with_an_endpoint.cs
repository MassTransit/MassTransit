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
namespace MassTransit.Transports.Tibco.Tests
{
	using Magnum.DateTimeExtensions;
	using MassTransit.Tests.Messages;
	using NUnit.Framework;
	using Serialization;

	[TestFixture]
	public class When_communicating_with_an_endpoint
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			LocalEndpointUri = "tibco://localhost:7222/mt_client";

			LocalEndpoint = new TibcoEndpoint(LocalEndpointUri, new XmlMessageSerializer());

			PurgeEndpoint();
		}

		#endregion

		private void PurgeEndpoint()
		{
			foreach (var selector in LocalEndpoint.SelectiveReceive(10.Milliseconds()))
			{
				selector.AcceptMessage();
			}
		}

		protected TibcoEndpoint LocalEndpoint { get; set; }
		protected string LocalEndpointUri { get; set; }

		[Test]
		public void Reading_a_single_message_should_return_one_message_selector()
		{
			LocalEndpoint.Send(new PingMessage());

			int count = 0;
			foreach (var selector in LocalEndpoint.SelectiveReceive(1.Seconds()))
			{
				selector.AcceptMessage();
				count++;
			}

			Assert.AreEqual(1, count);
		}

		[Test]
		public void Reading_from_an_empty_queue_should_just_return_an_empty_enumerator()
		{
			int count = 0;
			foreach (var selector in LocalEndpoint.SelectiveReceive(1.Seconds()))
			{
				selector.AcceptMessage();
				count++;
			}

			Assert.AreEqual(0, count);
		}

		[Test]
		public void Reading_without_receiving_should_return_the_same_set_of_messages()
		{
			LocalEndpoint.Send(new PingMessage());

			int count = 0;
			foreach (var selector in LocalEndpoint.SelectiveReceive(1.Seconds()))
			{
				object message = selector.DeserializeMessage();
				Assert.IsInstanceOfType(typeof (PingMessage), message);

				count++;
			}

			int secondCount = 0;
			foreach (var selector in LocalEndpoint.SelectiveReceive(1.Seconds()))
			{
				object message = selector.DeserializeMessage();
				Assert.IsInstanceOfType(typeof (PingMessage), message);

				secondCount++;
			}

			Assert.AreEqual(1, count);
			Assert.AreEqual(1, secondCount);
		}

		[Test]
		public void Receiving_the_message_and_accepting_it_should_make_it_go_away()
		{
			LocalEndpoint.Send(new PingMessage());

			foreach (var selector in LocalEndpoint.SelectiveReceive(1.Seconds()))
			{
				object message = selector.DeserializeMessage();
				Assert.IsInstanceOfType(typeof (PingMessage), message);

				Assert.IsTrue(selector.AcceptMessage());
			}

			int count = 0;
			foreach (var selector in LocalEndpoint.SelectiveReceive(1.Seconds()))
			{
				count++;
			}

			Assert.AreEqual(0, count);
		}
	}
}