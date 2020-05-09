namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using Common_Tests.ConsumeContextTestSubjects;
    using NUnit.Framework;


    public class Autofac_ConsumeContext :
        Common_ConsumeContext
    {
        readonly IContainer _container;

        public Autofac_ConsumeContext()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(pingTask);
            builder.RegisterInstance(sendEndpointProviderTask);
            builder.RegisterInstance(publishEndpointTask);

            builder.RegisterType<Service>().As<IService>().InstancePerLifetimeScope();
            builder.AddMassTransit(ConfigureRegistration);

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _container.DisposeAsync();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.Resolve<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.Resolve<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.Resolve<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }


    public class Autofac_ConsumeContext_Outbox :
        Common_ConsumeContext_Outbox
    {
        readonly IContainer _container;

        public Autofac_ConsumeContext_Outbox()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(pingTask);
            builder.RegisterInstance(sendEndpointProviderTask);
            builder.RegisterInstance(publishEndpointTask);

            builder.RegisterType<Service>().As<IService>().InstancePerLifetimeScope();
            builder.AddMassTransit(ConfigureRegistration);

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _container.DisposeAsync();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.Resolve<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.Resolve<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.Resolve<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }


    public class Autofac_ConsumeContext_Outbox_Solo :
        Common_ConsumeContext_Outbox_Solo
    {
        readonly IContainer _container;

        public Autofac_ConsumeContext_Outbox_Solo()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(pingTask);
            builder.RegisterInstance(sendEndpointProviderTask);
            builder.RegisterInstance(publishEndpointTask);
            builder.AddMassTransit(ConfigureRegistration);

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _container.DisposeAsync();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.Resolve<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.Resolve<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.Resolve<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }
}
