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
            configurator.ConfigureConsumer<SimpleConsumer>(_provider);
            configurator.ConfigureConsumer<SimplerConsumer>(_provider);
        }

        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            SimpleConsumer lastConsumer = await SimpleConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            SimpleMessageInterface last = await lastConsumer.Last;
            last.Name
                .ShouldBe(name);

            var wasDisposed = await lastConsumer.Dependency.WasDisposed;
            wasDisposed
                .ShouldBe(true); //Dependency was not disposed");

            lastConsumer.Dependency.SomethingDone
                .ShouldBe(true); //Dependency was disposed before consumer executed");

            SimplerConsumer lasterConsumer = await SimplerConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lasterConsumer.ShouldNotBe(null);

            SimpleMessageInterface laster = await lasterConsumer.Last.OrCanceled(TestCancellationToken);

        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseServiceScope(_provider);

            ConfigureConsumer(configurator);
        }
    }
}
