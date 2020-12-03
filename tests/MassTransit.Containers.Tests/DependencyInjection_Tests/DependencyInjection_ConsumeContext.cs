namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using System.Threading.Tasks;
    using Common_Tests;
    using Common_Tests.ConsumeContextTestSubjects;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using UnitOfWorkComponents;


    public class DependencyInjection_ConsumeContext :
        Common_ConsumeContext
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_ConsumeContext()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _provider = new ServiceCollection()
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<IAnotherService, AnotherService>()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
        protected override Task<ConsumeContext> ConsumeContext => _provider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _provider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _provider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (_provider is IDisposable disposable)
                disposable.Dispose();
        }
    }


    public class DependencyInjection_ConsumeContext_Outbox :
        Common_ConsumeContext_Outbox
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_ConsumeContext_Outbox()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _provider = new ServiceCollection()
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<IAnotherService, AnotherService>()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
        protected override Task<ConsumeContext> ConsumeContext => _provider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _provider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _provider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (_provider is IDisposable disposable)
                disposable.Dispose();
        }
    }


    public class DependencyInjection_ConsumeContext_Outbox_Batch :
        Common_ConsumeContext_Outbox_Batch
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_ConsumeContext_Outbox_Batch()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _provider = new ServiceCollection()
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<UnitOfWork>()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
        protected override Task<ConsumeContext> ConsumeContext => _provider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _provider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _provider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        protected override void ConfigureUnitOfWork(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseUnitOfWork();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (_provider is IDisposable disposable)
                disposable.Dispose();
        }
    }


    public class DependencyInjection_ConsumeContext_Filter_Batch :
        Common_ConsumeContext_Filter_Batch
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_ConsumeContext_Filter_Batch()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _provider = new ServiceCollection()
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<UnitOfWork>()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
        protected override Task<ConsumeContext> ConsumeContext => _provider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _provider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _provider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        protected override void ConfigureUnitOfWork(IInMemoryReceiveEndpointConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseConsumeFilter(configurator, typeof(UnitOfWorkFilter<>), Registration);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (_provider is IDisposable disposable)
                disposable.Dispose();
        }
    }


    namespace UnitOfWorkComponents
    {
        using ConsumeConfigurators;
        using GreenPipes;
        using GreenPipes.Specifications;


        public class UnitOfWorkFilter<TMessage> :
            IFilter<ConsumeContext<TMessage>>
            where TMessage : class
        {
            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("uow");
            }

            public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
            {
                var provider = context.GetPayload<IServiceProvider>();
                var unitOfWork = provider.GetRequiredService<UnitOfWork>();

                await next.Send(context);
            }
        }


        public class UnitOfWorkFilter<TContext, TConsumer> :
            IFilter<TContext>
            where TConsumer : class
            where TContext : class, ConsumerConsumeContext<TConsumer>
        {
            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("uow");
            }

            public async Task Send(TContext context, IPipe<TContext> next)
            {
                var provider = context.GetPayload<IServiceProvider>();
                var unitOfWork = provider.GetRequiredService<UnitOfWork>();

                await next.Send(context);
            }
        }


        public class UnitOfWorkConfigurationObserver :
            IConsumerConfigurationObserver
        {
            public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
                where TConsumer : class
            {
                var filter = new UnitOfWorkFilter<ConsumerConsumeContext<TConsumer>, TConsumer>();
                var specification = new FilterPipeSpecification<ConsumerConsumeContext<TConsumer>>(filter);
                configurator.AddPipeSpecification(specification);
            }

            public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
                where TConsumer : class
                where TMessage : class
            {
            }
        }


        public static class UnitOfWorkMiddlewareConfiguratorExtensions
        {
            public static void UseUnitOfWork(this IConsumePipeConfigurator configurator)
            {
                configurator.ConnectConsumerConfigurationObserver(new UnitOfWorkConfigurationObserver());
            }
        }
    }


    public class DependencyInjection_ConsumeContext_Outbox_Solo :
        Common_ConsumeContext_Outbox_Solo
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_ConsumeContext_Outbox_Solo()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _provider = new ServiceCollection()
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
        protected override Task<ConsumeContext> ConsumeContext => _provider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _provider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _provider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (_provider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
