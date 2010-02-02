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
	using Magnum;
	using Magnum.Actors;
	using Magnum.DateTimeExtensions;
	using NUnit.Framework;
	using TestFramework;
	using TestFramework.Examples.Messages;

	[Scenario]
	public class When_a_ping_is_published :
		Given_a_pong_service
	{
		[When]
		public void A_ping_is_published()
		{
			Message = new Ping(CombGuid.Generate());
			Response = new Future<Pong>();

			LocalBus.MakeRequest(bus => bus.Publish(Message, context => context.SendResponseTo(bus)))
				.When<Pong>().RelatedTo(Message.CorrelationId).IsReceived(message =>
					{
						// tag the future as received
						Response.Complete(message);
					})
				.TimeoutAfter(5.Seconds())
				.OnTimeout(() => { Assert.Fail("A response was not received"); })
				.Send();
		}

		[Then]
		public void The_response_should_have_been_received()
		{
			ExtensionMethodsForAssertions.ShouldBeTrue(Response.IsAvailable());
		}

		protected Ping Message { get; private set; }
		protected Future<Pong> Response { get; private set; }
	}
}