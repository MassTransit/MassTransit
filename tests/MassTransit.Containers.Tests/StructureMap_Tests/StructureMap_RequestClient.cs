namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using StructureMap;


    [TestFixture]
    public class StructureMap_RequestClient_Context
        : Common_RequestClient_Context
    {
        readonly IContainer _container;

        public StructureMap_RequestClient_Context()
        {
            _container = new Container(collection =>
            {
                collection.AddMassTransit(ConfigureRegistration);

                collection.For<IConsumeMessageObserver<InitialRequest>>().Use(context => GetConsumeObserver<InitialRequest>());
            });
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.GetInstance<IRequestClient<InitialRequest>>();
        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class StructureMap_RequestClient_Outbox
        : Common_RequestClient_Outbox
    {
        readonly IContainer _container;

        public StructureMap_RequestClient_Outbox()
        {
            _container = new Container(collection =>
            {
                collection.AddMassTransit(ConfigureRegistration);

                collection.For<IConsumeMessageObserver<InitialRequest>>().Use(context => GetConsumeObserver<InitialRequest>());
            });
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.GetInstance<IRequestClient<InitialRequest>>();
        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class StructureMap_RequestClient_Outbox_Courier
        : Common_RequestClient_Outbox_Courier
    {
        readonly IContainer _container;

        public StructureMap_RequestClient_Outbox_Courier()
        {
            _container = new Container(collection =>
            {
                collection.AddMassTransit(ConfigureRegistration);
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }
}
