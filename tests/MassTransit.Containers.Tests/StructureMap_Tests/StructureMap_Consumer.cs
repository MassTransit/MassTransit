namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios;
    using StructureMap;


    [TestFixture]
    public class StructureMap_Consumer :
        Common_Consumer
    {
        readonly IContainer _container;

        public StructureMap_Consumer()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<SimpleConsumer>();
                    cfg.AddBus(context => BusControl);
                });

                expression.For<ISimpleConsumerDependency>()
                    .Use<SimpleConsumerDependency>();

                expression.For<AnotherMessageConsumer>()
                    .Use<AnotherMessageConsumerImpl>();
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class StructureMap_Consumer_ConfigureEndpoint :
        Common_Consumer_ConfigureEndpoint
    {
        readonly IContainer _container;

        public StructureMap_Consumer_ConfigureEndpoint()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(ConfigureRegistration);

                expression.For<IConfigureReceiveEndpoint>()
                    .Add<DoNotPublishFaults>();
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class StructureMap_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly IContainer _container;

        public StructureMap_Consumer_Endpoint()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<SimplerConsumer>()
                        .Endpoint(e => e.Name = "custom-endpoint-name");

                    cfg.AddBus(context => BusControl);
                });

                expression.For<ISimpleConsumerDependency>()
                    .Use<SimpleConsumerDependency>();

                expression.For<AnotherMessageConsumer>()
                    .Use<AnotherMessageConsumerImpl>();
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class StructureMap_Consumer_Connect :
        Common_Consumer_Connect
    {
        readonly IContainer _container;

        public StructureMap_Consumer_Connect()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(ConfigureRegistration);

                expression.For<TaskCompletionSource<ConsumeContext<EasyMessage>>>()
                    .Use(MessageCompletion);
            });
        }

        protected override IReceiveEndpointConnector Connector => _container.GetInstance<IReceiveEndpointConnector>();
    }
}
