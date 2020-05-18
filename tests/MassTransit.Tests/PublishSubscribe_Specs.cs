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
namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            var message = new PingMessage();
            await Bus.Publish(message);

            await _received;
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_object :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            object message = new PingMessage();
            await Bus.Publish(message);

            await _received;
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_a_message_with_context :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            var message = new PingMessage();
            await Bus.Publish(message, Pipe.New<PublishContext>(x => x.UseExecute(v => v.RequestId = _requestId)));

            ConsumeContext<PingMessage> context = await _received;

            context.RequestId.HasValue.ShouldBe(true);
            context.RequestId.Value.ShouldBe(_requestId);
        }

        Task<ConsumeContext<PingMessage>> _received;
        readonly Guid _requestId = NewId.NextGuid();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_a_message_with_context_type :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            var message = new PingMessage();
            await Bus.Publish(message, Pipe.New<PublishContext<PingMessage>>(x => x.UseExecute(v => v.RequestId = _requestId)));

            ConsumeContext<PingMessage> context = await _received;

            context.RequestId.HasValue.ShouldBe(true);
            context.RequestId.Value.ShouldBe(_requestId);
        }

        Task<ConsumeContext<PingMessage>> _received;
        readonly Guid _requestId = NewId.NextGuid();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_object_with_context :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            object message = new PingMessage();
            await Bus.Publish(message, (PublishContext context) => context.RequestId = _requestId);

            ConsumeContext<PingMessage> consumeContext = await _received;

            consumeContext.RequestId.ShouldBe(_requestId);
        }

        Task<ConsumeContext<PingMessage>> _received;
        readonly Guid _requestId = NewId.NextGuid();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_interface_with_object :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            await Bus.Publish<PingMessage>(new { });

            ConsumeContext<PingMessage> context = await _received;
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_interface_with_object_and_context :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            await Bus.Publish<PingMessage>(new { }, v => v.RequestId = _requestId);

            ConsumeContext<PingMessage> context = await _received;

            context.RequestId.HasValue.ShouldBe(true);
            context.RequestId.ShouldBe(_requestId);
        }

        Task<ConsumeContext<PingMessage>> _received;
        readonly Guid _requestId = NewId.NextGuid();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }
}