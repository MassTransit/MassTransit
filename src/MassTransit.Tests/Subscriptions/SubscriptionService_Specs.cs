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
namespace MassTransit.Tests.Subscriptions
{
    using System;
    using System.Diagnostics;
    using Magnum.Extensions;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Inspectors;
    using Messages;
    using NUnit.Framework;
    using TestConsumers;
    using TestFramework;
    using TextFixtures;

    [TestFixture]
    public class SubscriptionService_Specs :
        LoopbackLocalAndRemoteTestFixture
    {
        void DumpPipelines()
        {
            Trace.WriteLine("LocalBus.InboundPipeline");
            PipelineViewer.Trace(LocalBus.InboundPipeline);

            Trace.WriteLine("LocalBus.OutboundPipeline");
            PipelineViewer.Trace(LocalBus.OutboundPipeline);

            Trace.WriteLine("RemoteBus.InboundPipeline");
            PipelineViewer.Trace(RemoteBus.InboundPipeline);

            Trace.WriteLine("RemoteBus.OutboundPipeline");
            PipelineViewer.Trace(RemoteBus.OutboundPipeline);
        }


        [Test]
        public void The_system_should_be_ready_to_use_before_getting_underway()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            ConnectHandle unsubscribeAction = RemoteBus.SubscribeInstance(consumer);

            LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();

            var message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, 8.Seconds());

            unsubscribeAction.Disconnect();
        }
    }
}