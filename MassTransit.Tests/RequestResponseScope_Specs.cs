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
namespace MassTransit.Tests
{
	using System.Threading;
	using Magnum.Common.DateTimeExtensions;
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
			FutureMessage<PongMessage> ponged = new FutureMessage<PongMessage>();

			LocalBus.Subscribe<PingMessage>(x => LocalBus.Publish(new PongMessage(x.CorrelationId)));

			PingMessage ping = new PingMessage();

			LocalBus.MakeRequest(bus => bus.Publish(ping, context => context.SendResponseTo(bus)))
				.When<PongMessage>().RelatedTo(ping.CorrelationId).IsReceived(pong =>
					{
						Assert.AreEqual(ping.CorrelationId, pong.CorrelationId);
						ponged.Set(pong);
					})
				.TimeoutAfter(5.Seconds())
				.Send();

			Assert.IsTrue(ponged.IsAvailable(1.Seconds()), "No response received");
		}

		[Test]
		public void A_timeout_handler_should_be_supported()
		{
			bool called = false;

			PingMessage ping = new PingMessage();

			LocalBus.MakeRequest(bus => bus.Publish(ping, context => context.SendResponseTo(bus)))
				.When<PongMessage>().RelatedTo(ping.CorrelationId).IsReceived(pong =>
					{
						Assert.Fail("Should not have gotten a response");
					})
				.TimeoutAfter(1.Seconds())
				.OnTimeout(()=>
					{
						called = true;
					})
				.Send();

			Assert.IsTrue(called, "Did not receive timeout invoker");
		}

		[Test]
		public void Any_type_of_send_should_be_supported()
		{
			RemoteBus.Subscribe<PingMessage>(x => RemoteBus.Publish(new PongMessage(x.CorrelationId)));

			PingMessage ping = new PingMessage();

			FutureMessage<PongMessage> ponged = new FutureMessage<PongMessage>();

			LocalBus.MakeRequest(bus => RemoteBus.Endpoint.Send(ping, context => context.SendResponseTo(bus)))
				.When<PongMessage>().RelatedTo(ping.CorrelationId).IsReceived(pong =>
					{
						Assert.AreEqual(ping.CorrelationId, pong.CorrelationId);
						ponged.Set(pong);
					})
				.TimeoutAfter(5.Seconds())
				.Send();

			Assert.IsTrue(ponged.IsAvailable(1.Seconds()), "No response received");
		}

		[Test]
		public void An_asynchronous_model_should_be_supported()
		{
			PingMessage ping = new PingMessage();

			ManualResetEvent mre = new ManualResetEvent(false);

			FutureMessage<PongMessage> ponged = new FutureMessage<PongMessage>();

			LocalBus.MakeRequest(bus => LocalBus.Endpoint.Send(ping, context => context.SendResponseTo(bus)))
				.When<PongMessage>().RelatedTo(ping.CorrelationId).IsReceived(pong =>
					{
						ponged.Set(pong);
					})
				.TimeoutAfter(5.Seconds())
				.OnTimeout(() => mre.Set())
				.BeginSend((state) => mre.Set(), null);

			LocalBus.Subscribe<PingMessage>(x => LocalBus.Publish(new PongMessage(x.CorrelationId)));

			Assert.IsTrue(mre.WaitOne(5.Seconds(), true));
			Assert.IsTrue(ponged.IsAvailable(1.Seconds()));
		}

		[Test]
		public void When_an_asynchronous_timeout_occurs_we_should_get_our_timeout_action_called()
		{
			PingMessage ping = new PingMessage();

			ManualResetEvent mre = new ManualResetEvent(false);

			FutureMessage<PongMessage> ponged = new FutureMessage<PongMessage>();

			LocalBus.MakeRequest(bus => LocalBus.Endpoint.Send(ping, context => context.SendResponseTo(bus)))
				.When<PongMessage>().RelatedTo(ping.CorrelationId).IsReceived(pong =>
					{
						ponged.Set(pong);
					})
				.OnTimeout(() =>
					{
						mre.Set();
					})
				.TimeoutAfter(2.Seconds())
				.BeginSend((state) => { }, null);

			Assert.IsTrue(mre.WaitOne(10.Seconds(), true));
			Assert.IsFalse(ponged.IsAvailable(0.Seconds()));
		}
	}
}