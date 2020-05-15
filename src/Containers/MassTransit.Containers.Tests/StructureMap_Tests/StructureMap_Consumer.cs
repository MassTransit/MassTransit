namespace MassTransit.Containers.Tests.StructureMap_Tests
{
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

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
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

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }
}
