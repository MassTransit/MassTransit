namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using Components;
    using Contracts;
    using LongRunningRequestTest;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class A_long_running_state_machine_initiated_by_a_request :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_accept_that_faults_happen()
        {
            IRequestClient<CreateShortLink> client = Bus.CreateRequestClient<CreateShortLink>(RequestTimeout.After(s: 8));

            Assert.That(async () => await client.GetResponse<ShortLinkCreated>(new { Link = new Uri("http://www.google.com") }),
                Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_complete_the_request()
        {
            IRequestClient<CreateShortLink> client = Bus.CreateRequestClient<CreateShortLink>(RequestTimeout.After(s: 8));

            Response<ShortLinkCreated> response = await client.GetResponse<ShortLinkCreated>(new { Link = new Uri("http://www.microsoft.com") });

            Console.WriteLine("Link: {0}, Short Link: {1}", response.Message.Link, response.Message.ShortLink);
        }

        RequestStateMachine _machine;
        CreateLinkStateMachine _createLinkStateMachine;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _createLinkStateMachine = new CreateLinkStateMachine();

            configurator.UseInMemoryOutbox();

            configurator.StateMachineSaga(_createLinkStateMachine, new InMemorySagaRepository<CreateLinkState>());

            EndpointConvention.Map<CreateShortLink>(configurator.InputAddress);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            _machine = new RequestStateMachine();

            configurator.ReceiveEndpoint("request-state", endpoint =>
            {
                var partitioner = endpoint.CreatePartitioner(128);

                endpoint.StateMachineSaga(_machine, new InMemorySagaRepository<RequestState>(), x =>
                {
                    x.Message<RequestStarted>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId));
                    x.Message<RequestCompleted>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId));
                    x.Message<RequestFaulted>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId));
                });
            });

            configurator.ReceiveEndpoint("short-link-service", endpoint =>
            {
                endpoint.Consumer<ShortLinkConsumer>();

                EndpointConvention.Map<RequestShortLink>(endpoint.InputAddress);
            });
        }
    }


    namespace LongRunningRequestTest
    {
        using System;


        public interface CreateShortLink
        {
            Uri Link { get; }
        }


        public interface RequestShortLink
        {
            Uri Link { get; }
        }


        public interface ShortLinkCreated
        {
            Uri Link { get; }
            Uri ShortLink { get; }
        }


        public class CreateLinkState :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }

            public Uri Link { get; set; }
            public Uri ShortLink { get; set; }

            public Guid? LinkRequestId { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class CreateLinkStateMachine :
            MassTransitStateMachine<CreateLinkState>
        {
            public CreateLinkStateMachine()
            {
                Event(() => CreateRequested, x =>
                {
                    x.CorrelateBy((instance, context) => instance.Link == context.Message.Link);
                    x.SelectId(context => context.RequestId ?? NewId.NextGuid());
                });

                Request(() => LinkRequest, x => x.LinkRequestId, x => x.Timeout = TimeSpan.Zero);

                During(Initial,
                    When(CreateRequested)
                        .Then(context => context.Instance.Link = context.Data.Link)
                        .Request(LinkRequest, x => x.Init<RequestShortLink>(new { x.Data.Link }))
                        .RequestStarted()
                        .TransitionTo(LinkRequest.Pending));

                During(LinkRequest.Pending,
                    When(LinkRequest.Completed)
                        .Then(context => context.Instance.ShortLink = context.Data.Link)
                        .RequestCompleted()
                        .TransitionTo(Valid),
                    When(LinkRequest.Faulted)
                        .RequestFaulted(CreateRequested)
                        .TransitionTo(Invalid));

                During(Valid,
                    When(CreateRequested)
                        .RespondAsync(x => x.Init<ShortLinkCreated>(new
                        {
                            x.Instance.Link,
                            x.Instance.ShortLink
                        })));
            }

            public State Valid { get; private set; }
            public State Invalid { get; private set; }

            public Event<CreateShortLink> CreateRequested { get; private set; }

            public Request<CreateLinkState, RequestShortLink, ShortLinkCreated> LinkRequest { get; private set; }
        }


        class ShortLinkConsumer :
            IConsumer<RequestShortLink>
        {
            public async Task Consume(ConsumeContext<RequestShortLink> context)
            {
                if (context.Message.Link == new Uri("http://www.google.com"))
                    throw new ArgumentException("Google is not valid", nameof(context.Message.Link));

                await context.RespondAsync<ShortLinkCreated>(new
                {
                    context.Message.Link,
                    ShortLink = context.Message.Link
                });
            }
        }
    }
}
