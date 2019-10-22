namespace MassTransit.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Definition;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestService;
    using TestService.Contracts;


    public class Linking_a_service_endpoint :
        InMemoryTestFixture
    {
        string _serviceEndpointName;
        Task<ConsumeContext<Up<DeployPayload>>> _upHandled;

        [Test]
        public async Task Should_publish_up_when_started()
        {
            var result = await _upHandled;

            Assert.That(result.Message.ServiceAddress.AbsolutePath.Trim('/'), Is.EqualTo(_serviceEndpointName));
        }

        [Test]
        public async Task Should_find_the_service()
        {
            var upHandler = SubscribeHandler<Up<DeployPayload>>();

            Guid clientId = NewId.NextGuid();

            await Bus.Publish<Link<DeployPayload>>(new
            {
                __ResponseAddress = Bus.Address,
                ClientId = clientId
            });

            var response = await upHandler;

            Assert.That(response.Message.ServiceAddress.AbsolutePath.Trim('/'), Is.EqualTo(_serviceEndpointName));
        }

        [Test]
        public async Task Should_find_and_invoke_the_service()
        {
            var upHandler = SubscribeHandler<Up<DeployPayload>>();

            Guid clientId = NewId.NextGuid();

            await Bus.Publish<Link<DeployPayload>>(new
            {
                __ResponseAddress = Bus.Address,
                __TimeToLive = 10000,
                ClientId = clientId
            });

            var upContext = await upHandler;

            var client = Bus.CreateRequestClient<DeployPayload>(upContext.Message.ServiceAddress);

            var request = client.Create(new
            {
                Target = "Bogey",
            });

            request.UseExecute(x => x.Headers.Set(MessageHeaders.ClientId, clientId));

            var response = await request.GetResponse<PayloadDeployed>();

            Assert.That(response.Message.Target, Is.EqualTo("Bogey"));
        }

        [Test]
        public async Task Should_fault_if_clientId_is_not_specified()
        {
            var upHandler = SubscribeHandler<Up<DeployPayload>>();

            Guid clientId = NewId.NextGuid();

            await Bus.Publish<Link<DeployPayload>>(new
            {
                __ResponseAddress = Bus.Address,
                __TimeToLive = 10000,
                ClientId = clientId
            });

            var upContext = await upHandler;

            var client = Bus.CreateRequestClient<DeployPayload>(upContext.Message.ServiceAddress);

            Assert.That(async () => await client.GetResponse<PayloadDeployed>(new {Target = "Bogey"}), Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_connect_using_the_service_client()
        {
            var serviceClient = Bus.CreateServiceClient();

            var requestClient = serviceClient.CreateRequestClient<DeployPayload>();

            var response = await requestClient.GetResponse<PayloadDeployed>(new {Target = "Bogey"});

            Assert.That(response.Message.Target, Is.EqualTo("Bogey"));
        }

        [Test]
        public async Task Should_timeout_if_a_service_is_not_found()
        {
            var serviceClient = Bus.CreateServiceClient();

            var requestClient = serviceClient.CreateRequestClient<DeployHappiness>(RequestTimeout.After(s: 2));

            Stopwatch timer = Stopwatch.StartNew();

            Assert.That(async () => await requestClient.GetResponse<PayloadDeployed>(new {Target = "Sunshine and Rainbows"}),
                Throws.TypeOf<RequestTimeoutException>());

            Assert.That(timer.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public async Task Should_default_timeout_if_a_service_is_not_found()
        {
            var serviceClient = Bus.CreateServiceClient(RequestTimeout.After(s: 2));

            var requestClient = serviceClient.CreateRequestClient<DeployHappiness>();

            Stopwatch timer = Stopwatch.StartNew();

            Assert.That(async () => await requestClient.GetResponse<PayloadDeployed>(new {Target = "Sunshine and Rainbows"}),
                Throws.TypeOf<RequestTimeoutException>());

            Assert.That(timer.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public async Task Should_timeout_via_cancellation_if_a_service_is_not_found()
        {
            var serviceClient = Bus.CreateServiceClient();

            var requestClient = serviceClient.CreateRequestClient<DeployHappiness>();

            Stopwatch timer = Stopwatch.StartNew();

            using (var token = new CancellationTokenSource(TimeSpan.FromSeconds(2)))
            {
                Assert.That(async () => await requestClient.GetResponse<PayloadDeployed>(new {Target = "Sunshine and Rainbows"}, token.Token),
                    Throws.TypeOf<RequestCanceledException>());
            }

            Assert.That(timer.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _upHandled = Handled<Up<DeployPayload>>(configurator);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _serviceEndpointName = KebabCaseEndpointNameFormatter.Instance.Consumer<DeployPayloadConsumer>();

            configurator.ServiceInstance(instance =>
            {
                instance.ReceiveEndpoint(_serviceEndpointName, x =>
                {
                    x.Consumer<DeployPayloadConsumer>();
                });
            });
        }
    }


    public class Linking_a_service_with_the_new_syntax :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_connect_using_the_service_client()
        {
            var serviceClient = Bus.CreateServiceClient();

            var requestClient = serviceClient.CreateRequestClient<DeployPayload>();

            var response = await requestClient.GetResponse<PayloadDeployed>(new {Target = "Bogey"});

            Assert.That(response.Message.Target, Is.EqualTo("Bogey"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ServiceInstance(instance =>
            {
                var serviceEndpointName = KebabCaseEndpointNameFormatter.Instance.Consumer<DeployPayloadConsumer>();

                instance.ReceiveEndpoint(serviceEndpointName, x =>
                {
                    x.Consumer<DeployPayloadConsumer>();
                });
            });
        }
    }


    [TestFixture]
    public class Linking_a_service_that_later_gets_a_second_instance
    {
        [Test]
        public async Task Should_work()
        {
            var bus = await CreateClientBus();
            try
            {
                var instanceA = await CreateInstance();
                try
                {
                    var serviceClient = bus.CreateServiceClient();

                    var requestClient = serviceClient.CreateRequestClient<DeployPayload>();

                    var response = await requestClient.GetResponse<PayloadDeployed>(new {Target = "A"});

                    Assert.That(response.Message.Target, Is.EqualTo("A"));

                    var instanceB = await CreateInstance();
                    try
                    {
                        await instanceA.StopAsync();

                        response = await requestClient.GetResponse<PayloadDeployed>(new {Target = "B"});

                        Assert.That(response.Message.Target, Is.EqualTo("B"));
                    }
                    finally
                    {
                        await instanceB.StopAsync();
                    }
                }
                finally
                {
                    //                    await instanceA.StopAsync();
                }
            }
            finally
            {
                await bus.StopAsync();
            }
        }

        static async Task<IBusControl> CreateInstance()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ServiceInstance(instance =>
                {
                    var serviceEndpointName = KebabCaseEndpointNameFormatter.Instance.Consumer<DeployPayloadConsumer>();

                    instance.ReceiveEndpoint(serviceEndpointName, x =>
                    {
                        x.Consumer<DeployPayloadConsumer>();
                    });
                });
            });

            await bus.StartAsync();

            return bus;
        }

        static async Task<IBusControl> CreateClientBus()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
            });

            await bus.StartAsync();

            return bus;
        }
    }


    namespace TestService
    {
        using Context;
        using Contracts;


        namespace Contracts
        {
            using System;


            public interface DeployHappiness
            {
                string Target { get; }
            }


            public interface DeployPayload
            {
                string Target { get; }
            }


            public interface PayloadDeployed
            {
                DateTime Timestamp { get; }
                string Target { get; }
            }
        }


        public class DeployPayloadConsumer :
            IConsumer<DeployPayload>
        {
            public Task Consume(ConsumeContext<DeployPayload> context)
            {
                LogContext.Info?.Log("Deploying Payload: {Target}", context.Message.Target);

                return context.RespondAsync<PayloadDeployed>(new
                {
                    InVar.Timestamp,
                    context.Message.Target
                });
            }
        }
    }
}
