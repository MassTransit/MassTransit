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
namespace MassTransit.Tests.Conventional
{
    using System;
    using System.Threading.Tasks;
    using ConsumeConnectors;
    using Internals.Extensions;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    [TestFixture]
    public class Configuring_a_consumer_by_custom_convention :
        InMemoryTestFixture
    {
        TaskCompletionSource<MessageA> _receivedA;
        TaskCompletionSource<MessageB> _receivedB;


        [TearDown]
        public Task TearDown()
        {
            ConsumerConvention.Remove<CustomConsumerConvention>();
            return TaskUtil.Completed;
        }


        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _receivedA = GetTask<MessageA>();
            _receivedB = GetTask<MessageB>();

            ConsumerConvention.Register<CustomConsumerConvention>();

            configurator.Consumer(typeof(CustomHandler), type => new CustomHandler(_receivedA, _receivedB));
        }


        class CustomHandler :
            IHandler<MessageA>,
            IHandler<MessageB>
        {
            readonly TaskCompletionSource<MessageA> _receivedA;
            readonly TaskCompletionSource<MessageB> _receivedB;

            public CustomHandler(TaskCompletionSource<MessageA> receivedA, TaskCompletionSource<MessageB> receivedB)
            {
                _receivedA = receivedA;
                _receivedB = receivedB;
            }

            public void Handle(MessageA message)
            {
                _receivedA.TrySetResult(message);
            }

            public void Handle(MessageB message)
            {
                _receivedB.TrySetResult(message);
            }
        }


        public interface MessageA
        {
            string Value { get; }
        }


        public interface MessageB
        {
            string Name { get; }
        }


        [Test]
        public async Task Should_find_the_message_handlers()
        {

            await Bus.Publish<MessageA>(new {Value = "Hello"});
            await Bus.Publish<MessageB>(new {Name = "World"});

            await _receivedA.Task;
            await _receivedB.Task;
        }
    }

    [TestFixture]
    public class Configuring_a_consumer_by_default_conventions :
    InMemoryTestFixture
    {
        TaskCompletionSource<MessageA> _receivedA;
        TaskCompletionSource<MessageB> _receivedB;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _receivedA = GetTask<MessageA>();
            _receivedB = GetTask<MessageB>();

            configurator.Consumer(typeof(DefaultConventionHandlers), type => new DefaultConventionHandlers(_receivedA, _receivedB));
        }


        class DefaultConventionHandlers :
            IConsumer<MessageA>,
            IMessageConsumer<MessageB>
        {
            readonly TaskCompletionSource<MessageA> _receivedA;
            readonly TaskCompletionSource<MessageB> _receivedB;

            public DefaultConventionHandlers(TaskCompletionSource<MessageA> receivedA, TaskCompletionSource<MessageB> receivedB)
            {
                _receivedA = receivedA;
                _receivedB = receivedB;
            }

            public Task Consume(ConsumeContext<MessageA> context)
            {
                _receivedA.TrySetResult(context.Message);
                return Task.FromResult(0);

            }

            public void Consume(MessageB message)
            {
                _receivedB.TrySetResult(message);
            }
        }


        public interface MessageA
        {
            string Value { get; }
        }


        public interface MessageB
        {
            string Name { get; }
        }


        [Test]
        public async Task Should_find_the_message_handlers()
        {

            await Bus.Publish<MessageA>(new { Value = "Hello" });
            await Bus.Publish<MessageB>(new { Name = "World" });

            await _receivedA.Task;
            await _receivedB.Task;
        }
    }

    [TestFixture]
    public class Configuring_a_consumer_without_a_matching_convention :
    InMemoryTestFixture
    {
        TaskCompletionSource<MessageA> _receivedA;
        TaskCompletionSource<MessageB> _receivedB;


        [TearDown]
        public Task TearDown()
        {
            ConsumerConvention.Register<AsyncConsumerConvention>();
            ConsumerConvention.Register<LegacyConsumerConvention>();
            return TaskUtil.Completed;
        }


        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _receivedA = GetTask<MessageA>();
            _receivedB = GetTask<MessageB>();

            ConsumerConvention.Remove<AsyncConsumerConvention>();
            ConsumerConvention.Remove<LegacyConsumerConvention>();

            configurator.Consumer(typeof(DefaultHandlers), type => new DefaultHandlers(_receivedA, _receivedB));
        }


        class DefaultHandlers :
            IConsumer<MessageA>,
            IMessageConsumer<MessageB>
        {
            readonly TaskCompletionSource<MessageA> _receivedA;
            readonly TaskCompletionSource<MessageB> _receivedB;

            public DefaultHandlers(TaskCompletionSource<MessageA> receivedA, TaskCompletionSource<MessageB> receivedB)
            {
                _receivedA = receivedA;
                _receivedB = receivedB;
            }

            public Task Consume(ConsumeContext<MessageA> context)
            {
                _receivedA.TrySetResult(context.Message);
                return Task.FromResult(0);

            }

            public void Consume(MessageB message)
            {
                _receivedB.TrySetResult(message);
            }
        }


        public interface MessageA
        {
            string Value { get; }
        }


        public interface MessageB
        {
            string Name { get; }
        }


        [Test]
        public async Task Should_not_find_the_message_handlers()
        {
            await Bus.Publish<MessageA>(new { Value = "Hello" });
            await Bus.Publish<MessageB>(new { Name = "World" });

            Assert.That(async () => await _receivedA.Task.WithTimeout(TimeSpan.FromSeconds(3)), Throws.TypeOf<TaskCanceledException>());
            Assert.That(async () => await _receivedB.Task.WithTimeout(TimeSpan.FromSeconds(3)), Throws.TypeOf<TaskCanceledException>());
        }
    }
}