// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public abstract class RequestClient_Context :
        InMemoryTestFixture
    {
        Guid _correlationId;

        public RequestClient_Context()
        {
            SubsequentQueueName = "subsequent_queue";
            SubsequentQueueAddress = new Uri(BaseAddress, SubsequentQueueName);
        }

        protected Uri SubsequentQueueAddress { get; }
        string SubsequentQueueName { get; }

        protected abstract IRequestClient<InitialRequest> RequestClient { get; }

        [Test]
        public async Task Should_receive_the_response()
        {
            //            Bus.ConnectAutofacConsumeMessageObserver<InitialRequest>(_container);

            var client = RequestClient;

            _correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new {CorrelationId = _correlationId, Value = "World"});

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World"));
            Assert.That(response.ConversationId.Value, Is.EqualTo(response.Message.OriginalConversationId));
            Assert.That(response.InitiatorId.Value, Is.EqualTo(_correlationId));
            Assert.That(response.Message.OriginalInitiatorId, Is.EqualTo(_correlationId));

            // var observer = _container.Resolve<TestConsumeMessageObserver<InitialRequest>>();
            //
            // await observer.PostConsumed;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(SubsequentQueueName, ConfigureSubsequentConsumer);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            ConfigureInitialConsumer(configurator);
        }

        protected abstract void ConfigureInitialConsumer(IInMemoryReceiveEndpointConfigurator configurator);
        protected abstract void ConfigureSubsequentConsumer(IInMemoryReceiveEndpointConfigurator configurator);


        protected class InitialConsumer :
            IConsumer<InitialRequest>
        {
            readonly IRequestClient<SubsequentRequest> _client;

            public InitialConsumer(IRequestClient<SubsequentRequest> client)
            {
                _client = client;
            }

            public async Task Consume(ConsumeContext<InitialRequest> context)
            {
                Response<SubsequentResponse> response = await _client.GetResponse<SubsequentResponse>(context.Message);

                await context.RespondAsync<InitialResponse>(response.Message);
            }
        }


        protected class SubsequentConsumer :
            IConsumer<SubsequentRequest>
        {
            public Task Consume(ConsumeContext<SubsequentRequest> context)
            {
                return context.RespondAsync<SubsequentResponse>(new
                {
                    OriginalConversationId = context.ConversationId.Value,
                    OriginalInitiatorId = context.InitiatorId.Value,
                    Value = $"Hello, {context.Message.Value}"
                });
            }
        }


        public interface InitialRequest
        {
            Guid CorrelationId { get; }
            string Value { get; }
        }


        public interface InitialResponse
        {
            Guid OriginalConversationId { get; }
            Guid OriginalInitiatorId { get; }
            string Value { get; }
        }


        public interface SubsequentRequest
        {
            Guid CorrelationId { get; }
            string Value { get; }
        }


        public interface SubsequentResponse
        {
            Guid OriginalConversationId { get; }
            Guid OriginalInitiatorId { get; }
            string Value { get; }
        }
    }
}