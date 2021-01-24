namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using NUnit.Framework;


    [TestFixture]
    public class Lamar_RequestClient_Context
        : Common_RequestClient_Context
    {
        readonly IContainer _container;

        public Lamar_RequestClient_Context()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(ConfigureRegistration);

                registry.For<IConsumeMessageObserver<InitialRequest>>().Use(context => GetConsumeObserver<InitialRequest>());
            });
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.GetInstance<IRequestClient<InitialRequest>>();
        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Lamar_RequestClient_Outbox
        : Common_RequestClient_Outbox
    {
        readonly IContainer _container;

        public Lamar_RequestClient_Outbox()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(ConfigureRegistration);

                registry.For<IConsumeMessageObserver<InitialRequest>>().Use(context => GetConsumeObserver<InitialRequest>());
            });
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.GetInstance<IRequestClient<InitialRequest>>();
        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Lamar_RequestClient_Outbox_Courier
        : Common_RequestClient_Outbox_Courier
    {
        readonly IContainer _container;

        public Lamar_RequestClient_Outbox_Courier()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(ConfigureRegistration);
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }
}
