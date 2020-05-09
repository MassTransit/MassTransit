namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using System.Threading.Tasks;
    using Common_Tests;
    using Common_Tests.ConsumeContextTestSubjects;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


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
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (_provider is IDisposable disposable)
                disposable.Dispose();
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _provider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _provider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _provider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;
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
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (_provider is IDisposable disposable)
                disposable.Dispose();
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _provider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _provider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _provider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;
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

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (_provider is IDisposable disposable)
                disposable.Dispose();
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _provider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _provider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _provider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }
}
