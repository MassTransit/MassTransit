// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Specifications;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Configuring_a_message_in_a_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_all_the_stuff()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _consumer.Received;

            await _message.Task;

            await _consumerOnly.Task;

            await _consumerMessage.Task;
        }

        MyConsumer _consumer;
        TaskCompletionSource<PingMessage> _message;
        TaskCompletionSource<Tuple<MyConsumer, PingMessage>> _consumerMessage;
        TaskCompletionSource<MyConsumer> _consumerOnly;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<PingMessage>());

            _message = GetTask<PingMessage>();
            _consumerOnly = GetTask<MyConsumer>();
            _consumerMessage = GetTask<Tuple<MyConsumer, PingMessage>>();

            configurator.Consumer(() => _consumer, cfg =>
            {
                cfg.UseExecute(context => _consumerOnly.TrySetResult(context.Consumer));
                cfg.Message<PingMessage>(m => m.UseExecute(context => _message.TrySetResult(context.Message)));
                cfg.ConsumerMessage<PingMessage>(m => m.UseExecute(context => _consumerMessage.TrySetResult(Tuple.Create(context.Consumer, context.Message))));
            });
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<PingMessage> _received;

            public MyConsumer(TaskCompletionSource<PingMessage> received)
            {
                _received = received;
            }

            public Task<PingMessage> Received => _received.Task;

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                _received.TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }
    }


    [TestFixture]
    public class Configuring_a_message_in_an_instance :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_all_the_stuff()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _consumer.Received;

            await _message.Task;

            await _consumerOnly.Task;

            await _consumerMessage.Task;
        }

        MyConsumer _consumer;
        TaskCompletionSource<PingMessage> _message;
        TaskCompletionSource<Tuple<MyConsumer, PingMessage>> _consumerMessage;
        TaskCompletionSource<MyConsumer> _consumerOnly;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<PingMessage>());

            _message = GetTask<PingMessage>();
            _consumerOnly = GetTask<MyConsumer>();
            _consumerMessage = GetTask<Tuple<MyConsumer, PingMessage>>();

            configurator.Instance(_consumer, cfg =>
            {
                cfg.UseExecute(context => _consumerOnly.TrySetResult(context.Consumer));
                cfg.Message<PingMessage>(m => m.UseExecute(context => _message.TrySetResult(context.Message)));
                cfg.ConsumerMessage<PingMessage>(m => m.UseExecute(context => _consumerMessage.TrySetResult(Tuple.Create(context.Consumer, context.Message))));
            });
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<PingMessage> _received;

            public MyConsumer(TaskCompletionSource<PingMessage> received)
            {
                _received = received;
            }

            public Task<PingMessage> Received => _received.Task;

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                _received.TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }
    }


    [TestFixture]
    public class Filters_on_the_send_pipeline :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_be_called_once()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _consumer.Received;

            Assert.That(_sendFilter.Count, Is.EqualTo(1));
        }

        MyConsumer _consumer;
        SendFilter _sendFilter;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _sendFilter = new SendFilter();

            configurator.ConfigureSend(pc => pc.AddPipeSpecification(_sendFilter));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<PingMessage>());

            configurator.Instance(_consumer);
        }


        public class SendFilter :
            IFilter<SendContext>,
            IPipeSpecification<SendContext>
        {
            int _count;

            public int Count => _count;

            void IPipeSpecification<SendContext>.Apply(IPipeBuilder<SendContext> builder)
            {
                builder.AddFilter(this);
            }

            void IProbeSite.Probe(ProbeContext context)
            {
            }

            Task IFilter<SendContext>.Send(SendContext context, IPipe<SendContext> next)
            {
                Interlocked.Increment(ref _count);

                return next.Send(context);
            }

            IEnumerable<ValidationResult> ISpecification.Validate()
            {
                yield break;
            }
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<PingMessage> _received;

            public MyConsumer(TaskCompletionSource<PingMessage> received)
            {
                _received = received;
            }

            public Task<PingMessage> Received => _received.Task;

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                _received.TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }
    }

    [TestFixture]
    public class Filters_on_the_publish_pipeline :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_be_called_once()
        {
            await Bus.Publish(new PingMessage());

            await _consumer.Received;

            Assert.That(_sendFilter.Count, Is.EqualTo(1));
        }

        MyConsumer _consumer;
        SendFilter _sendFilter;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _sendFilter = new SendFilter();

            configurator.ConfigurePublish(pc => pc.AddPipeSpecification(_sendFilter));

            configurator.ConfigurePublish(pc =>
                pc.AddPipeSpecification(new DelegatePipeSpecification<PublishContext<PingMessage>>(context =>
            {
            })));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<PingMessage>());

            configurator.Instance(_consumer);
        }


        public class SendFilter :
            IFilter<SendContext>,
            IPipeSpecification<SendContext>
        {
            int _count;

            public int Count => _count;

            void IPipeSpecification<SendContext>.Apply(IPipeBuilder<SendContext> builder)
            {
                builder.AddFilter(this);
            }

            void IProbeSite.Probe(ProbeContext context)
            {
            }

            Task IFilter<SendContext>.Send(SendContext context, IPipe<SendContext> next)
            {
                Interlocked.Increment(ref _count);

                return next.Send(context);
            }

            IEnumerable<ValidationResult> ISpecification.Validate()
            {
                yield break;
            }
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<PingMessage> _received;

            public MyConsumer(TaskCompletionSource<PingMessage> received)
            {
                _received = received;
            }

            public Task<PingMessage> Received => _received.Task;

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                _received.TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }
    }


    [TestFixture]
    public class Filters_on_the_send_pipeline_for_nested_messages :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_be_called_once()
        {
            await InputQueueSendEndpoint.Send(new Message());

            await _consumer.Received;

            Assert.That(_sendFilter.Count, Is.EqualTo(1));
        }

        MyConsumer _consumer;
        SendFilter _sendFilter;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _sendFilter = new SendFilter();

            configurator.ConfigureSend(pc => pc.AddPipeSpecification(_sendFilter));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<Message>());

            configurator.Instance(_consumer);
        }


        class SendFilter :
            IFilter<SendContext>,
            IPipeSpecification<SendContext>
        {
            int _count;

            public int Count => _count;

            void IPipeSpecification<SendContext>.Apply(IPipeBuilder<SendContext> builder)
            {
                builder.AddFilter(this);
            }

            void IProbeSite.Probe(ProbeContext context)
            {
            }

            Task IFilter<SendContext>.Send(SendContext context, IPipe<SendContext> next)
            {
                Interlocked.Increment(ref _count);

                return next.Send(context);
            }

            IEnumerable<ValidationResult> ISpecification.Validate()
            {
                yield break;
            }
        }


        public abstract class ParentMessage
        {
        }


        public class Message :
            ParentMessage
        {
        }


        class MyConsumer :
            IConsumer<Message>
        {
            readonly TaskCompletionSource<Message> _received;

            public MyConsumer(TaskCompletionSource<Message> received)
            {
                _received = received;
            }

            public Task<Message> Received => _received.Task;

            public Task Consume(ConsumeContext<Message> context)
            {
                _received.TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }
    }

    [TestFixture]
    public class Filters_on_the_publish_pipeline_for_nested_messages :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_be_called_once()
        {
            await Bus.Publish(new Message());

            await _consumer.Received;

            Assert.That(_sendFilter.Count, Is.EqualTo(1));
        }

        MyConsumer _consumer;
        SendFilter _sendFilter;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _sendFilter = new SendFilter();

            configurator.ConfigurePublish(pc => pc.AddPipeSpecification(_sendFilter));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<Message>());

            configurator.Instance(_consumer);
        }


        class SendFilter :
            IFilter<SendContext>,
            IPipeSpecification<SendContext>
        {
            int _count;

            public int Count => _count;

            void IPipeSpecification<SendContext>.Apply(IPipeBuilder<SendContext> builder)
            {
                builder.AddFilter(this);
            }

            void IProbeSite.Probe(ProbeContext context)
            {
            }

            Task IFilter<SendContext>.Send(SendContext context, IPipe<SendContext> next)
            {
                Interlocked.Increment(ref _count);

                return next.Send(context);
            }

            IEnumerable<ValidationResult> ISpecification.Validate()
            {
                yield break;
            }
        }


        public abstract class ParentMessage
        {
        }


        public class Message :
            ParentMessage
        {
        }


        class MyConsumer :
            IConsumer<Message>
        {
            readonly TaskCompletionSource<Message> _received;

            public MyConsumer(TaskCompletionSource<Message> received)
            {
                _received = received;
            }

            public Task<Message> Received => _received.Task;

            public Task Consume(ConsumeContext<Message> context)
            {
                _received.TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }
    }
}