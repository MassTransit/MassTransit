namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using TestFramework.Courier;


    [TestFixture]
    public class WindsorCourier_ExecuteActivity :
        Courier_ExecuteActivity
    {
        readonly IWindsorContainer _container;

        public WindsorCourier_ExecuteActivity()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddExecuteActivity<SetVariableActivity, SetVariableArguments>();
                x.AddBus(provider => BusControl);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class WindsorCourier_ExecuteActivity_Endpoint :
        Courier_ExecuteActivity_Endpoint
    {
        readonly IWindsorContainer _container;

        public WindsorCourier_ExecuteActivity_Endpoint()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddExecuteActivity<SetVariableActivity, SetVariableArguments>()
                    .Endpoint(e => e.Name = "custom-setvariable-execute");

                x.AddBus(provider => BusControl);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class WindsorCourier_Activity :
        Courier_Activity
    {
        readonly IWindsorContainer _container;

        public WindsorCourier_Activity()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddActivity<TestActivity, TestArguments, TestLog>();
                x.AddBus(provider => BusControl);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class WindsorCourier_Activity_Endpoint :
        Courier_Activity_Endpoint
    {
        readonly IWindsorContainer _container;

        public WindsorCourier_Activity_Endpoint()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddActivity<TestActivity, TestArguments, TestLog>()
                    .Endpoints(e => e.Name = "custom-testactivity-execute", e => e.Name = "custom-testactivity-compensate");

                x.AddBus(provider => BusControl);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }
}
