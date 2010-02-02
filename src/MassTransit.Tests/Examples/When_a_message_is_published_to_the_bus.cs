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
namespace MassTransit.Tests.Examples
{
	using Magnum.DateTimeExtensions;
	using Messages;
	using TestFramework;

	[Scenario]
	public class When_a_message_is_published_to_the_bus :
		Given_a_consumer_is_subscribed_to_a_message
	{
		[When]
		public void A_message_is_published_to_the_bus()
		{
			Message = new SimpleMessage();

			LocalBus.Publish(Message);
		}

		protected SimpleMessage Message { get; private set; }

		[Then]
		public void The_consumer_should_receive_the_message()
		{
			Consumer.ShouldHaveReceived(Message, 1.Seconds());
		}
	}
}