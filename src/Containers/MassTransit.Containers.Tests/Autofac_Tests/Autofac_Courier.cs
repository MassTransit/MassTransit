namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;
    using TestFramework.Courier;


    [TestFixture]
    public class AutofacCourier_ExecuteActivity :
        Courier_ExecuteActivity
    {
        readonly IContainer _container;

        public AutofacCourier_ExecuteActivity()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>();
                cfg.AddBus(context => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
    }


    [TestFixture]
    public class AutofacCourier_ExecuteActivity_Endpoint :
        Courier_ExecuteActivity_Endpoint
    {
        readonly IContainer _container;

        public AutofacCourier_ExecuteActivity_Endpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>()
                    .Endpoint(e => e.Name = "custom-setvariable-execute");

                cfg.AddBus(context => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
    }


    [TestFixture]
    public class AutofacCourier_Activity :
        Courier_Activity
    {
        readonly IContainer _container;

        public AutofacCourier_Activity()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>();
                cfg.AddBus(context => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
    }


    [TestFixture]
    public class AutofacCourier_Activity_Endpoint :
        Courier_Activity_Endpoint
    {
        readonly IContainer _container;

        public AutofacCourier_Activity_Endpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>()
                    .Endpoints(e => e.Name = "custom-testactivity-execute", e => e.Name = "custom-testactivity-compensate");

                cfg.AddBus(context => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
    }
}
