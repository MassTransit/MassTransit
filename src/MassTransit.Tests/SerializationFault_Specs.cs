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
    using System.Linq;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_a_message_fails_to_deserialize_properly :
        InMemoryTestFixture
    {
        [Test]
        public void It_should_respond_with_a_serialization_fault()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestFaultException>());
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;
        Task<PongMessage> _response;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage, PongMessage>();

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                throw new SerializationException("This is fine, forcing death");
            });
        }
    }

#if !NETCORE

    [TestFixture]
    public class When_a_message_response_fails_to_serialize_properly_and_is_using_the_binary_serializer :
        InMemoryTestFixture
    {
        [Serializable]
        public class PingMessage2 { }


        public class PongMessage2
        {
        }

        Task<ConsumeContext<PingMessage2>> _handled;
        Task<ConsumeContext<ReceiveFault>> _faulted;

        [Test]
        public async Task It_should_respond_with_a_fault_indicating_that_the_type_could_not_be_serialized()
        {
            var faultContext = await _faulted;

            Assert.That(faultContext.Message.Exceptions.First().Message.Contains("PongMessage2"));
        }

        IRequestClient<PingMessage2, PongMessage2> _requestClient;
        Task<PongMessage2> _response;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage2, PongMessage2>();

            _response = _requestClient.Request(new PingMessage2());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseBinarySerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage2>(configurator, async context =>
            {
                context.Respond(new PongMessage2());
            });

            _faulted = Handled<ReceiveFault>(configurator);
        }
    }

    #endif

    /// <summary>
    /// this requires debugger tricks to make it work
    /// </summary>
    [TestFixture, Explicit]
    public class When_a_message_has_an_unrecognized_body_format :
        InMemoryTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handled;
        Task<ConsumeContext<ReceiveFault>> _faulted;

        [Test]
        public async Task It_should_publish_a_fault()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), context => context.ContentType = new ContentType("text/json"));

            var faultContext = await _faulted;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);

            _faulted = Handled<ReceiveFault>(configurator);
        }
    }
}