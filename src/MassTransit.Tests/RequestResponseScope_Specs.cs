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
namespace MassTransit.Tests
{
	using System;
	using System.Threading;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using Messages;
	using NUnit.Framework;
	using TextFixtures;


	[TestFixture]
	public class When_creating_a_simple_request_response_handler :
		LoopbackLocalAndRemoteTestFixture
	{
		[Test]
		public void A_clean_method_of_a_request_reply_should_be_possible()
		{
			var ponged = new FutureMessage<PongMessage>();

			RemoteBus.SubscribeHandler<PingMessage>(x => RemoteBus.Publish(new PongMessage(x.CorrelationId)));

			var ping = new PingMessage();


			LocalBus.PublishRequest(ping, x =>
				{
					x.Handle<PongMessage>(message =>
						{
							message.CorrelationId.ShouldEqual(ping.CorrelationId);
							ponged.Set(message);
						});

					x.HandleTimeout(5.Seconds(), () => { });
				});

			ponged.IsAvailable(8.Seconds()).ShouldBeTrue("No ping response received");
		}
	}
}