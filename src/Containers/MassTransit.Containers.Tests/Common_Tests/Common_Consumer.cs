namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Definition;
    using GreenPipes.Internals.Extensions;
    using NUnit.Framework;
    using Scenarios;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    public abstract class Common_Consumer :
        InMemoryTestFixture
    {
        protected abstract IRegistration Registration { get; }

        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimpleConsumer.LastConsumer;
            lastConsumer.ShouldNotBe(null);

            var last = await lastConsumer.Last;
            last.Name
                .ShouldBe(name);

            var wasDisposed = await lastConsumer.Dependency.WasDisposed;
            wasDisposed
                .ShouldBe(true); //Dependency was not disposed");

            lastConsumer.Dependency.SomethingDone
                .ShouldBe(true); //Dependency was disposed before consumer executed");
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimpleConsumer>(Registration);
        }
    }


    public abstract class Common_Consumer_Endpoint :
        InMemoryTestFixture
    {
        protected abstract IRegistration Registration { get; }

        [Test]
        public async Task Should_receive_on_the_custom_endpoint()
        {
            const string name = "Joe";

            var sendEndpoint = await Bus.GetSendEndpoint(new Uri("loopback://localhost/custom-endpoint-name"));

            await sendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimplerConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            var last = await lastConsumer.Last.OrCanceled(TestCancellationToken);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }
    }


    public abstract class Common_Consumers_Endpoint :
        InMemoryTestFixture
    {
        protected abstract IRegistration Registration { get; }

        [Test]
        public async Task Should_receive_on_the_custom_endpoint()
        {
            IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>(new Uri("queue:shared"));

            await client.GetResponse<PongMessage>(new PingMessage());

            IRequestClient<Request> clientB = Bus.CreateRequestClient<Request>(new Uri("queue:shared"));

            await clientB.GetResponse<Response>(new Request());
        }

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddConsumer<ConsumerA>(typeof(ConsumerADefinition))
                .Endpoint(x => x.Name = "shared");

            configurator.AddConsumer<ConsumerB>(typeof(ConsumerBDefinition))
                .Endpoint(x => x.Name = "shared");

            configurator.AddConsumer<ConsumerC>(typeof(ConsumerCDefinition));

            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }


        class ConsumerA :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return TaskUtil.Completed;
            }
        }


        class ConsumerADefinition :
            ConsumerDefinition<ConsumerA>
        {
        }


        class ConsumerB :
            IConsumer<Request>
        {
            public Task Consume(ConsumeContext<Request> context)
            {
                return context.RespondAsync(new Response());
            }
        }


        class ConsumerBDefinition :
            ConsumerDefinition<ConsumerB>
        {
            public ConsumerBDefinition()
            {
                Endpoint(x => x.Name = "broken");
            }
        }


        class ConsumerC :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }


        class ConsumerCDefinition :
            ConsumerDefinition<ConsumerC>
        {
            public ConsumerCDefinition()
            {
                Endpoint(e => e.Name = "shared");
            }
        }


        class Request
        {
        }


        class Response
        {
        }
    }


    public abstract class Common_Consumer_ServiceEndpoint :
        InMemoryTestFixture
    {
        protected abstract IRegistration Registration { get; }

        [Test]
        public async Task Should_handle_the_request()
        {
            var serviceClient = Bus.CreateServiceClient();

            IRequestClient<PingMessage> requestClient = serviceClient.CreateRequestClient<PingMessage>();

            var pingId = NewId.NextGuid();

            Response<PongMessage> response = await requestClient.GetResponse<PongMessage>(new PingMessage(pingId));

            Assert.That(response.Message.CorrelationId, Is.EqualTo(pingId));
        }

        [Test]
        public void Should_just_startup()
        {
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ServiceInstance(x => x.ConfigureEndpoints(Registration));
        }
    }
}
