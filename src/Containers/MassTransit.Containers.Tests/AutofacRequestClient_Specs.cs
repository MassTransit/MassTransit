// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using NUnit.Framework;
    using TestFramework;
    using Testing.Observers;


    public class AutofacRequestClient_Specs :
        InMemoryTestFixture
    {
        IContainer _container;
        ILifetimeScope _initialContainer;
        ILifetimeScope _subsequentContainer;
        Guid _correlationId;

        public AutofacRequestClient_Specs()
        {
            SubsequentQueueName = "subsequent_queue";
            SubsequentQueueAddress = new Uri(BaseAddress, SubsequentQueueName);
        }

        Uri SubsequentQueueAddress { get; }
        string SubsequentQueueName { get; }

        [Test]
        public async Task Should_receive_the_response()
        {
            Bus.ConnectAutofacConsumeMessageObserver<InitialRequest>(_container);

            var client = _container.Resolve<IRequestClient<InitialRequest>>();

            _correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new {CorrelationId = _correlationId, Value = "World"});

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World"));
            Assert.That(response.ConversationId.Value, Is.EqualTo(response.Message.OriginalConversationId));
            Assert.That(response.InitiatorId.Value, Is.EqualTo(_correlationId));
            Assert.That(response.Message.OriginalInitiatorId, Is.EqualTo(_correlationId));

            var observer = _container.Resolve<TestConsumeMessageObserver<InitialRequest>>();

            await observer.PostConsumed;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var builder = new ContainerBuilder();

            builder.Register(context => Bus)
                .As<IBus>()
                .ExternallyOwned();

            builder.Register(context => GetConsumeObserver<InitialRequest>())
                .As<IConsumeMessageObserver<InitialRequest>>()
                .AsSelf()
                .SingleInstance();

            builder.Register(context => context.Resolve<IBus>().CreateClientFactory())
                .As<IClientFactory>();

            builder.Register(context => context.Resolve<IClientFactory>().CreateRequestClient<InitialRequest>(InputQueueAddress))
                .As<IRequestClient<InitialRequest>>();

            _container = builder.Build();

            _initialContainer = _container.BeginLifetimeScope(cfg =>
            {
                cfg.Register(context => context.Resolve<IBus>().CreateClientFactory())
                    .As<IClientFactory>();

                cfg.Register(context => context.Resolve<IClientFactory>().CreateRequestClient<SubsequentRequest>(context.Resolve<ConsumeContext>(),
                        SubsequentQueueAddress))
                    .As<IRequestClient<SubsequentRequest>>();

                cfg.RegisterType<InitialConsumer>();
            });

            _subsequentContainer = _container.BeginLifetimeScope(cfg =>
            {
                cfg.RegisterType<SubsequentConsumer>();
            });


            configurator.ReceiveEndpoint(SubsequentQueueName, cfg => cfg.LoadFrom(_subsequentContainer));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_initialContainer);
        }


        class InitialConsumer :
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


        class SubsequentConsumer :
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