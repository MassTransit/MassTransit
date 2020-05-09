namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using Common_Tests.ConsumeContextTestSubjects;
    using NUnit.Framework;
    using StructureMap;


    public class StructureMap_ConsumeContext :
        Common_ConsumeContext
    {
        readonly IContainer _container;

        public StructureMap_ConsumeContext()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> publishEndpointTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask = GetTask<ISendEndpointProvider>();

            _container = new Container(expression =>
            {
                expression.AddMassTransit(ConfigureRegistration);

                expression.For<TaskCompletionSource<ConsumeContext>>().Use(pingTask);
                expression.For<TaskCompletionSource<IPublishEndpoint>>().Use(publishEndpointTask);
                expression.For<TaskCompletionSource<ISendEndpointProvider>>().Use(sendEndpointProviderTask);

                expression.For<IService>().Use<Service>();
            });
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


    public class StructureMap_ConsumeContext_Outbox :
        Common_ConsumeContext_Outbox
    {
        readonly IContainer _container;

        public StructureMap_ConsumeContext_Outbox()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> publishEndpointTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask = GetTask<ISendEndpointProvider>();

            _container = new Container(expression =>
            {
                expression.AddMassTransit(ConfigureRegistration);

                expression.For<TaskCompletionSource<ConsumeContext>>().Use(pingTask);
                expression.For<TaskCompletionSource<IPublishEndpoint>>().Use(publishEndpointTask);
                expression.For<TaskCompletionSource<ISendEndpointProvider>>().Use(sendEndpointProviderTask);

                expression.For<IService>().Use<Service>();
            });
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


    public class StructureMap_ConsumeContext_Outbox_Solo :
        Common_ConsumeContext_Outbox_Solo
    {
        readonly IContainer _container;

        public StructureMap_ConsumeContext_Outbox_Solo()
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> publishEndpointTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask = GetTask<ISendEndpointProvider>();

            _container = new Container(expression =>
            {
                expression.AddMassTransit(ConfigureRegistration);

                expression.For<TaskCompletionSource<ConsumeContext>>().Use(pingTask);
                expression.For<TaskCompletionSource<IPublishEndpoint>>().Use(publishEndpointTask);
                expression.For<TaskCompletionSource<ISendEndpointProvider>>().Use(sendEndpointProviderTask);
            });
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
