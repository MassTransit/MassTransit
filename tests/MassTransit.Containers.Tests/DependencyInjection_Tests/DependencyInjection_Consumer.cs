namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using System.Threading.Tasks;
    using Common_Tests;
    using GreenPipes;
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

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
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

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
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

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
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
            services.AddSingleton(TaskCompletionSource);
            services.AddMassTransit(ConfigureRegistration);
            _provider = services.BuildServiceProvider(true);
        }

        protected override void ConfigureFilter(IConsumePipeConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjection_Consume_FilterScope :
        Common_Consume_FilterScope
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consume_FilterScope()
        {
            _provider = new ServiceCollection()
                .AddSingleton(EasyASource)
                .AddSingleton(EasyBSource)
                .AddSingleton(ScopedContextSource)
                .AddSingleton(AsyncTestHarness)
                .AddScoped<ScopedContext>()
                .AddScoped(typeof(ScopedConsumeFilter<>))
                .AddScoped(typeof(ScopedSendFilter<>))
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override void ConfigureFilters(IInMemoryReceiveEndpointConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedConsumeFilter<>), Registration);
            DependencyInjectionFilterExtensions.UseSendFilter(configurator, typeof(ScopedSendFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjection_Consumer_Connect :
        Common_Consumer_Connect
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consumer_Connect()
        {
            _provider = new ServiceCollection()
                .AddSingleton(MessageCompletion)
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override IReceiveEndpointConnector Connector => _provider.GetRequiredService<IReceiveEndpointConnector>();
    }


    [TestFixture]
    public class DependencyInjection_Consumer_FilterOrder :
        Common_Consumer_FilterOrder
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consumer_FilterOrder()
        {
            _provider = new ServiceCollection()
                .AddSingleton(MessageCompletion)
                .AddSingleton(ConsumerCompletion)
                .AddSingleton(ConsumerMessageCompletion)
                .AddSingleton<IFilter<ConsumeContext<EasyMessage>>, MessageFilter<EasyMessage, IServiceProvider>>()
                .AddSingleton<IFilter<ConsumerConsumeContext<EasyConsumer>>, ConsumerFilter<EasyConsumer, IServiceProvider>>()
                .AddSingleton<IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>>, ConsumerMessageFilter<EasyConsumer, EasyMessage, IServiceProvider>>()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();

        protected override IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>> CreateConsumerMessageFilter()
        {
            return _provider.GetRequiredService<IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>>>();
        }

        protected override IFilter<ConsumerConsumeContext<EasyConsumer>> CreateConsumerFilter()
        {
            return _provider.GetRequiredService<IFilter<ConsumerConsumeContext<EasyConsumer>>>();
        }

        protected override IFilter<ConsumeContext<EasyMessage>> CreateMessageFilter()
        {
            return _provider.GetRequiredService<IFilter<ConsumeContext<EasyMessage>>>();
        }
    }


    [TestFixture]
    public class DependencyInjection_Consumer_ScopedFilterOrder :
        Common_Consumer_ScopedFilterOrder<IServiceProvider>
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consumer_ScopedFilterOrder()
        {
            _provider = new ServiceCollection()
                .AddSingleton(MessageCompletion)
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);
            DependencyInjectionFilterExtensions.UseConsumeFilter(configurator, typeof(MessageFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();


        class MessageFilter<TMessage> :
            IFilter<ConsumeContext<TMessage>>
            where TMessage : class
        {
            readonly TaskCompletionSource<ConsumeContext<TMessage>> _taskCompletionSource;

            public MessageFilter(TaskCompletionSource<ConsumeContext<TMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
            {
                if (context.TryGetPayload(out IServiceProvider _))
                    _taskCompletionSource.TrySetResult(context);
                else
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider not found"));

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
