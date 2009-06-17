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
namespace MassTransit.Tests.Subscriptions
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Services.Subscriptions.Messages;
	using MassTransit.Transports;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestConsumers;
	using TextFixtures;

	[TestFixture]
	public class SubscriptionService_Specs :
		SubscriptionServiceTestFixture<LoopbackEndpoint>
	{
		private void DumpPipelines()
		{
			Trace.WriteLine("LocalBus.InboundPipeline");
			PipelineViewer.Trace(LocalBus.InboundPipeline);

			Trace.WriteLine("LocalBus.OutboundPipeline");
			PipelineViewer.Trace(LocalBus.OutboundPipeline);

			Trace.WriteLine("RemoteBus.InboundPipeline");
			PipelineViewer.Trace(RemoteBus.InboundPipeline);

			Trace.WriteLine("RemoteBus.OutboundPipeline");
			PipelineViewer.Trace(RemoteBus.OutboundPipeline);

			Trace.WriteLine("SubscriptionBus.InboundPipeline");
			PipelineViewer.Trace(SubscriptionBus.InboundPipeline);

			Trace.WriteLine("SubscriptionBus.OutboundPipeline");
			PipelineViewer.Trace(SubscriptionBus.OutboundPipeline);
		}

		[Test]
		public void Removing_a_subscription_twice_should_not_have_a_negative_impact()
		{
			Guid clientId = CombGuid.Generate();

			SubscriptionInformation subscription = new SubscriptionInformation(clientId, 1, typeof (PingMessage), RemoteBus.Endpoint.Uri);

			LocalControlBus.Endpoint.Send(new AddSubscription(subscription));
			Thread.Sleep(250);

			LocalControlBus.Endpoint.Send(new RemoveSubscription(subscription));
			LocalControlBus.Endpoint.Send(new RemoveSubscription(subscription));
			Thread.Sleep(250);

			PipelineViewer.Trace(LocalBus.OutboundPipeline);
		}

		[Test]
		public void The_initial_subscriptions_should_be_read_from_the_repository()
		{
			SubscriptionRepository.AssertWasCalled(x => x.List());
		}

		[Test]
		public void The_system_should_be_ready_to_use_before_getting_underway()
		{
			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
			var unsubscribeAction = RemoteBus.Subscribe(consumer);

			Thread.Sleep(1000);

			DumpPipelines();

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, 500.Milliseconds());

			unsubscribeAction();
		}
	}
}