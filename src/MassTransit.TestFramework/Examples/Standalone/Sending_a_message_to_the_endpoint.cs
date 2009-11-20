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
namespace MassTransit.TestFramework.Examples.Standalone
{
	using System.Diagnostics;
	using Magnum.DateTimeExtensions;
	using Messages;

	[Scenario]
	public class Sending_a_message_to_the_endpoint :
		Given_a_standalone_service_bus
	{
		[Given]
		public void A_consumer_is_subscribed_to_the_message()
		{
			Trace.WriteLine("Given");

			Consumer = new ConsumerOf<SimpleMessage>();
			LocalBus.Subscribe(Consumer);
		}

		[When]
		public void A_message_is_sent_to_the_bus_endpoint()
		{
			Trace.WriteLine("When");

			Message = new SimpleMessage();

			LocalBus.Endpoint.Send(Message);
		}

		[Then]
		public void The_consumer_should_receive_the_message()
		{
			Consumer.ShouldHaveReceivedMessage(Message, 1.Seconds());
		}

		[Then]
		public void This_should_be_true()
		{
			true.ShouldBeTrue();
		}

		[Then]
		public void This_should_be_false()
		{
			false.ShouldBeFalse();
		}

		protected SimpleMessage Message { get; private set; }
		protected ConsumerOf<SimpleMessage> Consumer { get; private set; }
	}
}