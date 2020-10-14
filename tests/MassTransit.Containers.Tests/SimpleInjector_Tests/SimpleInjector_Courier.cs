namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using SimpleInjector;
    using TestFramework.Courier;


    [TestFixture]
    public class SimpleInjectorCourier_ExecuteActivity :
        Courier_ExecuteActivity
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjectorCourier_ExecuteActivity()
        {
            _container = new Container();
            _container.SetRequiredOptions();
            _container.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>();
                cfg.AddBus(() => BusControl);
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class SimpleInjectorCourier_ExecuteActivity_Endpoint :
        Courier_ExecuteActivity_Endpoint
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjectorCourier_ExecuteActivity_Endpoint()
        {
            _container = new Container();
            _container.SetRequiredOptions();
            _container.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>()
                    .Endpoint(e => e.Name = "custom-setvariable-execute");

                cfg.AddBus(() => BusControl);
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class SimpleInjectorCourier_Activity :
        Courier_Activity
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjectorCourier_Activity()
        {
            _container = new Container();
            _container.SetRequiredOptions();
            _container.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>();
                cfg.AddBus(() => BusControl);
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }

    [TestFixture]
    public class SimpleInjectorCourier_Activity_Endpoint :
        Courier_Activity_Endpoint
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjectorCourier_Activity_Endpoint()
        {
            _container = new Container();
            _container.SetRequiredOptions();
            _container.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>()
                    .Endpoints(e => e.Name = "custom-testactivity-execute", e => e.Name = "custom-testactivity-compensate");

                cfg.AddBus(() => BusControl);
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }
}
