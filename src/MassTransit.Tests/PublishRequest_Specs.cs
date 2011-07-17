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
namespace MassTransit.Tests
{
	using System;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using Messages;
	using NUnit.Framework;
	using RequestResponse;
	using TextFixtures;

	[TestFixture]
	public class Publishing_a_simple_request :
		LoopbackLocalAndRemoteTestFixture
	{
		[Test]
		public void Should_use_a_clean_syntax_following_standard_conventions()
		{
			var pongReceived = new FutureMessage<PongMessage>();
			var pingReceived = new FutureMessage<PingMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(x =>
				{
					pingReceived.Set(x);
					RemoteBus.MessageContext<PingMessage>().Respond(new PongMessage(x.CorrelationId));
				});

			var ping = new PingMessage();

			var timeout = 8.Seconds();

			IRequest<PingMessage, Guid> request = LocalBus.PublishRequest(ping, x =>
				{
					x.Handle<PongMessage>(message =>
						{
							message.CorrelationId.ShouldEqual(ping.CorrelationId, "The response correlationId did not match");
							pongReceived.Set(message);
						});

					x.SetTimeout(timeout);
				});

			pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
			pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

			request.Wait().ShouldBeTrue("The request was not completed before the timeout expired");
		}
	}
}