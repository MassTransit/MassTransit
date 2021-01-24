namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;


    [TestFixture]
    public class Windsor_RequestClient_Context
        : Common_RequestClient_Context
    {
        readonly IWindsorContainer _container;

        public Windsor_RequestClient_Context()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(ConfigureRegistration);
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.Resolve<IRequestClient<InitialRequest>>();

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Windsor_RequestClient_Outbox
        : Common_RequestClient_Outbox
    {
        readonly IWindsorContainer _container;

        public Windsor_RequestClient_Outbox()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(ConfigureRegistration);
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.Resolve<IRequestClient<InitialRequest>>();

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Windsor_RequestClient_Outbox_Courier
        : Common_RequestClient_Outbox_Courier
    {
        readonly IWindsorContainer _container;

        public Windsor_RequestClient_Outbox_Courier()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(ConfigureRegistration);
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }
}
