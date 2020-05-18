namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class DependencyInjection_ServiceScope :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimpleConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            var last = await lastConsumer.Last;
            last.Name
                .ShouldBe(name);

            var wasDisposed = await lastConsumer.Dependency.WasDisposed;
            wasDisposed
                .ShouldBe(true); //Dependency was not disposed");

            lastConsumer.Dependency.SomethingDone
                .ShouldBe(true); //Dependency was disposed before consumer executed");

            var lasterConsumer = await SimplerConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lasterConsumer.ShouldNotBe(null);

            var laster = await lasterConsumer.Last.OrCanceled(TestCancellationToken);
        }

        readonly IServiceProvider _provider;

        public DependencyInjection_ServiceScope()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleConsumer>();
                x.AddConsumer<SimplerConsumer>();
                x.AddBus(provider => BusControl);
            });

            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            _provider = collection.BuildServiceProvider(true);
        }

        protected void ConfigureConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            var registration = _provider.GetRequiredService<IRegistration>();

            configurator.ConfigureConsumer<SimpleConsumer>(registration);
            configurator.ConfigureConsumer<SimplerConsumer>(registration);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseServiceScope(_provider);

            ConfigureConsumer(configurator);
        }
    }
}
