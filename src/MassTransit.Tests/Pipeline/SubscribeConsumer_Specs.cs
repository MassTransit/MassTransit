// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Diagnostics;
    using Magnum.Extensions;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using MassTransit.Pipeline.Inspectors;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TestConsumers;
    using TestFramework;


    [TestFixture]
    public class When_subscribing_a_consumer_to_the_pipeline
    {
        [SetUp]
        public void Setup()
        {
            _pipeline = InboundPipelineConfigurator.CreateDefault(null);
        }

        IInboundMessagePipeline _pipeline;

        [Test]
        public void A_bunch_of_mixed_subscriber_types_should_work()
        {
            var consumer = new IndiscriminantConsumer<PingMessage>();

            Stopwatch firstTime = Stopwatch.StartNew();
            var unsubscribeToken = _pipeline.ConnectInstance(consumer);
            firstTime.Stop();

            PipelineViewer.Trace(_pipeline);

            var message = new PingMessage();

            _pipeline.Dispatch(message);

            Assert.AreEqual(message, consumer.Consumed);

            unsubscribeToken();

            var nextMessage = new PingMessage();
            _pipeline.Dispatch(nextMessage);

            Assert.AreEqual(message, consumer.Consumed);
        }

        [Test]
        public void A_component_should_be_subscribed_to_multiple_messages_on_the_pipeline()
        {
            var consumer = MockRepository.GenerateMock<PingPongConsumer>();

            _pipeline.ConnectConsumer(() => consumer);

            PipelineViewer.Trace(_pipeline);

            var ping = new PingMessage();
            consumer.Expect(x => x.Consume(ping));
            _pipeline.Dispatch(ping);

            var pong = new PongMessage(ping.CorrelationId);
            consumer.Expect(x => x.Consume(pong));
            _pipeline.Dispatch(pong);

            consumer.VerifyAllExpectations();
        }

        [Test]
        public void A_component_should_be_subscribed_to_the_pipeline()
        {
            var consumer = new TestMessageConsumer<PingMessage>();

            _pipeline.ConnectConsumer(() => consumer);

            PipelineViewer.Trace(_pipeline);

            var message = new PingMessage();

            _pipeline.ShouldHaveSubscriptionFor<PingMessage>();

            _pipeline.Dispatch(message);

            TestMessageConsumer<PingMessage>.AnyShouldHaveReceivedMessage(message, 1.Seconds());
        }

        [Test]
        public void The_subscription_should_be_added()
        {
            var consumer = new IndiscriminantConsumer<PingMessage>();

            Stopwatch firstTime = Stopwatch.StartNew();
            _pipeline.ConnectInstance(consumer);
            firstTime.Stop();

            var message = new PingMessage();

            _pipeline.Dispatch(message);

            Assert.AreEqual(message, consumer.Consumed);
        }

        [Test]
        public void The_wrong_type_of_message_should_not_blow_up_the_test()
        {
            var consumer = new IndiscriminantConsumer<PingMessage>();

            _pipeline.ConnectInstance(consumer);

            var message = new PongMessage();

            _pipeline.Dispatch(message);

            Assert.AreEqual(null, consumer.Consumed);
        }
    }
}