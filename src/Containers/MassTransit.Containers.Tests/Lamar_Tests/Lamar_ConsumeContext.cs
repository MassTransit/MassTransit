namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using Common_Tests.ConsumeContextTestSubjects;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    public class Lamar_ConsumeContext :
        Common_ConsumeContext
    {
        readonly Container _container;

        public Lamar_ConsumeContext()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _container = new Container(registry =>
            {
                registry.AddSingleton(pingTask);
                registry.AddSingleton(sendEndpointProviderTask);
                registry.AddSingleton(publishEndpointTask);
                registry.AddScoped<IService, Service>();
                registry.AddMassTransit(ConfigureRegistration);
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;

        protected override Task<ISendEndpointProvider> SendEndpointProvider =>
            _container.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }


    public class Lamar_ConsumeContext_Outbox :
        Common_ConsumeContext_Outbox
    {
        readonly Container _container;

        public Lamar_ConsumeContext_Outbox()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _container = new Container(registry =>
            {
                registry.AddSingleton(pingTask);
                registry.AddSingleton(sendEndpointProviderTask);
                registry.AddSingleton(publishEndpointTask);
                registry.AddScoped<IService, Service>();
                registry.AddMassTransit(ConfigureRegistration);
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;

        protected override Task<ISendEndpointProvider> SendEndpointProvider =>
            _container.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }


    public class Lamar_ConsumeContext_Outbox_Solo :
        Common_ConsumeContext_Outbox_Solo
    {
        readonly Container _container;

        public Lamar_ConsumeContext_Outbox_Solo()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _container = new Container(registry =>
            {
                registry.AddSingleton(pingTask);
                registry.AddSingleton(sendEndpointProviderTask);
                registry.AddSingleton(publishEndpointTask);
                registry.AddMassTransit(ConfigureRegistration);
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;

        protected override Task<ISendEndpointProvider> SendEndpointProvider =>
            _container.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }
}
