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
    using Magnum.Extensions;
    using Messages;
	using NUnit.Framework;
	using TextFixtures;
    using TestFramework;

	[TestFixture]
	public class MessageQueueEndpoint_MeetsCriteria :
		LoopbackLocalAndRemoteTestFixture
	{
		private readonly PingMessage _message = new PingMessage();

		[Test]
		public void Subscring_to_an_endpoint_should_accept_and_dispatch_messages()
		{
            FutureMessage<PingMessage> fm=new FutureMessage<PingMessage>();
			bool workDid = false;

			LocalBus.SubscribeHandler<PingMessage>(
				(msg)=> {workDid = true; fm.Set(msg); });

			RemoteBus.ShouldHaveSubscriptionFor<PingMessage>();

			RemoteBus.Publish(_message);
		    fm.IsAvailable(1.Seconds());
			Assert.That(workDid, Is.True, "Lazy Test!");
		}
	}
}