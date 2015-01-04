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
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using NUnit.Framework;
    using Policies;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Connecting_a_consumer_to_the_inbound_pipe :
        MessageTestFixture
    {
        [Test]
        public async void Should_receive_a_message()
        {
            IConsumePipe filter = new ConsumePipe();

            OneMessageConsumer consumer = GetOneMessageConsumer();

            IConsumerFactory<OneMessageConsumer> factory = GetInstanceConsumerFactory(consumer);

            filter.ConnectConsumer(factory, retryPolicy: Retry.None);

            var consumeContext = new TestConsumeContext<MessageA>(new MessageA());

            await filter.Send(consumeContext);

            await consumer.Task;
        }

        [Test]
        public async void Should_receive_a_message_via_object()
        {
            IConsumePipe filter = new ConsumePipe();

            OneMessageConsumer consumer = GetOneMessageConsumer();

            object subscribeConsumer = consumer;

            filter.ConnectInstance(subscribeConsumer, Retry.None);

            var consumeContext = new TestConsumeContext<MessageA>(new MessageA());

            await filter.Send(consumeContext);

            await consumer.Task;
        }

        [Test, Explicit]
        public void Should_receive_a_message_pipeline_view()
        {
            IConsumePipe filter = new ConsumePipe();

            OneMessageConsumer consumer = GetOneMessageConsumer();

            IConsumerFactory<OneMessageConsumer> factory = GetInstanceConsumerFactory(consumer);

            filter.ConnectConsumer(factory, retryPolicy: Retry.None);

            var inspector = new StringPipeVisitor();
            filter.Visit(inspector);

            Console.WriteLine(inspector.ToString());
        }

        [Test]
        public async void Should_receive_a_two_messages()
        {
            IConsumePipe filter = new ConsumePipe();

            TwoMessageConsumer consumer = GetTwoMessageConsumer();

            IConsumerFactory<TwoMessageConsumer> factory = GetInstanceConsumerFactory(consumer);

            filter.ConnectConsumer(factory, retryPolicy: Retry.None);

            await filter.Send(new TestConsumeContext<MessageA>(new MessageA()));

            await filter.Send(new TestConsumeContext<MessageB>(new MessageB()));

            await consumer.TaskA;
            await consumer.TaskB;
        }
    }
}