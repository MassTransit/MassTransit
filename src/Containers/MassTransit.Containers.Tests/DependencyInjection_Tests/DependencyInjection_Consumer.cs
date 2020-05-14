namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;


    [TestFixture]
    public class DependencyInjection_Consumer :
        Common_Consumer
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consumer()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleConsumer>();
                x.AddBus(provider => BusControl);
            });

            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
    }


    [TestFixture]
    public class DependencyInjection_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consumer_Endpoint()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(provider => BusControl);
            });

            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            _provider = collection.BuildServiceProvider();
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
    }


    [TestFixture]
    public class DependencyInjection_Consumers_Endpoint :
        Common_Consumers_Endpoint
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consumers_Endpoint()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
    }


    [TestFixture]
    public class DependencyInjection_Consume_Filter :
        Common_Consume_Filter
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consume_Filter()
        {
            var services = new ServiceCollection();
            services.AddScoped(_ => new MyId(Guid.NewGuid()));
            services.AddScoped(typeof(ScopedFilter<>));
            services.AddSingleton(TaskCompletionSource);
            services.AddMassTransit(ConfigureRegistration);
            _provider = services.BuildServiceProvider(true);
        }

        protected override void ConfigureFilter(IConsumePipeConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedFilter<>), Registration);
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
    }
}
