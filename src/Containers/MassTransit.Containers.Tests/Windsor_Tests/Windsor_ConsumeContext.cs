namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using System.Threading.Tasks;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Common_Tests;
    using Common_Tests.ConsumeContextTestSubjects;
    using NUnit.Framework;


    public class Windsor_ConsumeContext :
        Common_ConsumeContext
    {
        readonly IWindsorContainer _container;

        public Windsor_ConsumeContext()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> publishEndpointTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask = GetTask<ISendEndpointProvider>();

            _container = new WindsorContainer()
                .AddMassTransit(ConfigureRegistration);

            _container.Register(Component.For<IService>().ImplementedBy<Service>().LifestyleScoped(),
                Component.For<TaskCompletionSource<ConsumeContext>>().Instance(pingTask),
                Component.For<TaskCompletionSource<IPublishEndpoint>>().Instance(publishEndpointTask),
                Component.For<TaskCompletionSource<ISendEndpointProvider>>().Instance(sendEndpointProviderTask)
            );
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override MassTransit.IRegistration Registration => _container.Resolve<MassTransit.IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.Resolve<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.Resolve<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.Resolve<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }


    public class Windsor_ConsumeContext_Outbox :
        Common_ConsumeContext_Outbox
    {
        readonly IWindsorContainer _container;

        public Windsor_ConsumeContext_Outbox()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> publishEndpointTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask = GetTask<ISendEndpointProvider>();

            _container = new WindsorContainer()
                .AddMassTransit(ConfigureRegistration);

            _container.Register(Component.For<IService>().ImplementedBy<Service>().LifestyleScoped(),
                Component.For<TaskCompletionSource<ConsumeContext>>().Instance(pingTask),
                Component.For<TaskCompletionSource<IPublishEndpoint>>().Instance(publishEndpointTask),
                Component.For<TaskCompletionSource<ISendEndpointProvider>>().Instance(sendEndpointProviderTask)
            );
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override MassTransit.IRegistration Registration => _container.Resolve<MassTransit.IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.Resolve<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.Resolve<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.Resolve<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }


    public class Windsor_ConsumeContext_Outbox_Solo :
        Common_ConsumeContext_Outbox_Solo
    {
        readonly IWindsorContainer _container;

        public Windsor_ConsumeContext_Outbox_Solo()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> publishEndpointTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask = GetTask<ISendEndpointProvider>();

            _container = new WindsorContainer()
                .AddMassTransit(ConfigureRegistration);

            _container.Register(Component.For<TaskCompletionSource<ConsumeContext>>().Instance(pingTask),
                Component.For<TaskCompletionSource<IPublishEndpoint>>().Instance(publishEndpointTask),
                Component.For<TaskCompletionSource<ISendEndpointProvider>>().Instance(sendEndpointProviderTask)
            );
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override MassTransit.IRegistration Registration => _container.Resolve<MassTransit.IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.Resolve<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.Resolve<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.Resolve<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }
}
