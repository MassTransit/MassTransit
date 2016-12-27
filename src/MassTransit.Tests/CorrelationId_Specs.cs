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
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_correlated_message :
        InMemoryTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }

        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var pingMessage = new PingMessage();
            await Bus.Publish(pingMessage);

            var context = await _handled;

            context.CorrelationId.HasValue.ShouldBe(true);
            context.CorrelationId.Value.ShouldBe(pingMessage.CorrelationId);
        }
    }


    [TestFixture]
    public class Sending_a_correlated_message :
        InMemoryTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }

        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var pingMessage = new PingMessage();

            await InputQueueSendEndpoint.Send(pingMessage);

            var context = await _handled;

            context.CorrelationId.HasValue.ShouldBe(true);
            context.CorrelationId.Value.ShouldBe(pingMessage.CorrelationId);
        }
    }


    [TestFixture]
    public class Sending_a_correlation_id_message :
        InMemoryTestFixture
    {
        Task<ConsumeContext<A>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        class A
        {
            public Guid CorrelationId { get; set; }
        }


        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A {CorrelationId = NewId.NextGuid()};

            await InputQueueSendEndpoint.Send(message);

            var context = await _handled;

            context.CorrelationId.HasValue.ShouldBe(true);
            context.CorrelationId.Value.ShouldBe(message.CorrelationId);
        }
    }

    [TestFixture]
    public class Sending_a_command_id_message :
        InMemoryTestFixture
    {
        Task<ConsumeContext<A>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        class A
        {
            public Guid CommandId { get; set; }
        }


        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A {CommandId = NewId.NextGuid()};

            await InputQueueSendEndpoint.Send(message);

            var context = await _handled;

            context.CorrelationId.HasValue.ShouldBe(true);
            context.CorrelationId.Value.ShouldBe(message.CommandId);
        }
    }

    [TestFixture]
    public class Sending_an_event_id_message :
        InMemoryTestFixture
    {
        Task<ConsumeContext<A>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        class A
        {
            public Guid EventId { get; set; }
            public Guid CommandId { get; set; }
        }


        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A {EventId = NewId.NextGuid()};

            await InputQueueSendEndpoint.Send(message);

            var context = await _handled;

            context.CorrelationId.HasValue.ShouldBe(true);
            context.CorrelationId.Value.ShouldBe(message.EventId);
        }
    }

    [TestFixture]
    public class Sending_a_nullable_correlation_id_message :
        InMemoryTestFixture
    {
        Task<ConsumeContext<A>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        class A
        {
            public Guid? CorrelationId { get; set; }
        }


        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A {CorrelationId = NewId.NextGuid()};

            await InputQueueSendEndpoint.Send(message);

            var context = await _handled;

            context.CorrelationId.HasValue.ShouldBe(true);
            context.CorrelationId.Value.ShouldBe(message.CorrelationId.Value);
        }
    }
}