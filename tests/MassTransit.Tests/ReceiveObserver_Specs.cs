// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    namespace ReceivingObserver_Specs
    {
        using System;
        using System.Threading.Tasks;
        using GreenPipes;
        using Metadata;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Messages;
        using Util;


        [TestFixture]
        public class Receiving_messages_at_the_endpoint :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_call_the_post_consume_notification()
            {
                ReceiveObserver observer = GetObserver();

                using (ConnectHandle observerHandle = Bus.ConnectReceiveObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    Tuple<ConsumeContext, string> context = await observer.PostConsumed;

                    Assert.AreEqual(TypeMetadataCache<MessageHandler<PingMessage>>.ShortName, context.Item2);
                }
            }

            [Test]
            public async Task Should_call_the_post_receive_notification()
            {
                ReceiveObserver observer = GetObserver();

                ConnectHandle observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.PostReceived;
            }

            [Test]
            public async Task Should_call_the_pre_receive_notification()
            {
                ReceiveObserver observer = GetObserver();

                ConnectHandle observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.PreReceived;
            }

            ReceiveObserver GetObserver()
            {
                return new ReceiveObserver(GetTask<ReceiveContext>(), GetTask<ReceiveContext>(), GetTask<Tuple<ReceiveContext, Exception>>(),
                    GetTask<Tuple<ConsumeContext, string>>(), GetTask<ConsumeContext>());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                Handled<PingMessage>(configurator);
            }
        }

        [TestFixture]
        public class Receiving_messages_at_the_endpoint_by_consumer :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_call_the_post_consume_notification()
            {
                ReceiveObserver observer = GetObserver();

                using (ConnectHandle observerHandle = Bus.ConnectReceiveObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    Tuple<ConsumeContext, string> context = await observer.PostConsumed;

                    Assert.AreEqual(TypeMetadataCache<PingConsumerDude>.ShortName, context.Item2);
                }
            }

            ReceiveObserver GetObserver()
            {
                return new ReceiveObserver(GetTask<ReceiveContext>(), GetTask<ReceiveContext>(), GetTask<Tuple<ReceiveContext, Exception>>(),
                    GetTask<Tuple<ConsumeContext, string>>(), GetTask<ConsumeContext>());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.Consumer<PingConsumerDude>();
            }


            class PingConsumerDude :
                IConsumer<PingMessage>
            {
                public async Task Consume(ConsumeContext<PingMessage> context)
                {
                }
            }
        }


        [TestFixture]
        public class Receiving_messages_at_the_endpoint_badly :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_call_the_pre_receive_notification()
            {
                ReceiveObserver observer = GetObserver();

                ConnectHandle observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.PreReceived;
            }

            [Test]
            public async Task Should_call_the_receive_fault_notification()
            {
                ReceiveObserver observer = GetObserver();

                ConnectHandle observerHandle = Bus.ConnectReceiveObserver(observer);

                await InputQueueSendEndpoint.Send(new PingMessage());

                await observer.ConsumeFaulted;
            }

            ReceiveObserver GetObserver()
            {
                return new ReceiveObserver(GetTask<ReceiveContext>(), GetTask<ReceiveContext>(), GetTask<Tuple<ReceiveContext, Exception>>(),
                    GetTask<Tuple<ConsumeContext, string>>(), GetTask<ConsumeContext>());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                Handler<PingMessage>(configurator, x =>
                {
                    throw new IntentionalTestException();
                });
            }
        }


        class ReceiveObserver :
            IReceiveObserver
        {
            readonly TaskCompletionSource<ConsumeContext> _consumeFault;
            readonly TaskCompletionSource<Tuple<ConsumeContext, string>> _postConsume;
            readonly TaskCompletionSource<ReceiveContext> _postReceive;
            readonly TaskCompletionSource<ReceiveContext> _preReceive;
            readonly TaskCompletionSource<Tuple<ReceiveContext, Exception>> _receiveFault;

            public ReceiveObserver(TaskCompletionSource<ReceiveContext> preReceive, TaskCompletionSource<ReceiveContext> postReceive,
                TaskCompletionSource<Tuple<ReceiveContext, Exception>> receiveFault, TaskCompletionSource<Tuple<ConsumeContext, string>> postConsume,
                TaskCompletionSource<ConsumeContext> consumeFault)
            {
                _preReceive = preReceive;
                _postReceive = postReceive;
                _receiveFault = receiveFault;
                _postConsume = postConsume;
                _consumeFault = consumeFault;
            }

            public Task<ReceiveContext> PreReceived
            {
                get { return _preReceive.Task; }
            }

            public Task<ReceiveContext> PostReceived
            {
                get { return _postReceive.Task; }
            }

            public Task<Tuple<ConsumeContext, string>> PostConsumed
            {
                get { return _postConsume.Task; }
            }

            public Task<ConsumeContext> ConsumeFaulted
            {
                get { return _consumeFault.Task; }
            }

            public Task<Tuple<ReceiveContext, Exception>> ReceiveFaulted
            {
                get { return _receiveFault.Task; }
            }

            public async Task PreReceive(ReceiveContext context)
            {
                _preReceive.TrySetResult(context);
            }

            public async Task PostReceive(ReceiveContext context)
            {
                _postReceive.TrySetResult(context);
            }

            public async Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
                where T : class
            {
                _postConsume.TrySetResult(Tuple.Create<ConsumeContext, string>(context, consumerType));
            }

            public async Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception) where T : class
            {
                _consumeFault.TrySetResult(context);
            }

            public async Task ReceiveFault(ReceiveContext context, Exception exception)
            {
                _receiveFault.TrySetResult(Tuple.Create(context, exception));
            }
        }
    }
}