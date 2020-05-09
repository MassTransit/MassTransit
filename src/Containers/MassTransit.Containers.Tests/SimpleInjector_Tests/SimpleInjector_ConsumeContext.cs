namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using Common_Tests.ConsumeContextTestSubjects;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjector_ConsumeContext :
        Common_ConsumeContext
    {
        readonly Container _container;

        public SimpleInjector_ConsumeContext()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.RegisterInstance(pingTask);
            _container.RegisterInstance(sendEndpointProviderTask);
            _container.RegisterInstance(publishEndpointTask);

            _container.Register<IService, Service>(Lifestyle.Scoped);
            _container.AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.GetInstance<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.GetInstance<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.GetInstance<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }


    public class SimpleInjector_ConsumeContext_Outbox :
        Common_ConsumeContext_Outbox
    {
        readonly Container _container;

        public SimpleInjector_ConsumeContext_Outbox()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.RegisterInstance(pingTask);
            _container.RegisterInstance(sendEndpointProviderTask);
            _container.RegisterInstance(publishEndpointTask);

            _container.Register<IService, Service>(Lifestyle.Scoped);
            _container.AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.GetInstance<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.GetInstance<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.GetInstance<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }


    public class SimpleInjector_ConsumeContext_Outbox_Solo :
        Common_ConsumeContext_Outbox_Solo
    {
        readonly Container _container;

        public SimpleInjector_ConsumeContext_Outbox_Solo()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.RegisterInstance(pingTask);
            _container.RegisterInstance(sendEndpointProviderTask);
            _container.RegisterInstance(publishEndpointTask);

            _container.AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
        protected override Task<ConsumeContext> ConsumeContext => _container.GetInstance<TaskCompletionSource<ConsumeContext>>().Task;
        protected override Task<IPublishEndpoint> PublishEndpoint => _container.GetInstance<TaskCompletionSource<IPublishEndpoint>>().Task;
        protected override Task<ISendEndpointProvider> SendEndpointProvider => _container.GetInstance<TaskCompletionSource<ISendEndpointProvider>>().Task;
    }
}
