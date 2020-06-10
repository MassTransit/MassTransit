namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Courier;


    [TestFixture]
    public class LamarCourier_ExecuteActivity :
        Courier_ExecuteActivity
    {
        readonly IContainer _container;

        public LamarCourier_ExecuteActivity()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>();
                    cfg.AddBus(context => BusControl);
                });
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class LamarCourier_ExecuteActivity_Endpoint :
        Courier_ExecuteActivity_Endpoint
    {
        readonly IContainer _container;

        public LamarCourier_ExecuteActivity_Endpoint()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>()
                        .Endpoint(e => e.Name = "custom-setvariable-execute");

                    cfg.AddBus(context => BusControl);
                });
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class LamarCourier_Activity :
        Courier_Activity
    {
        readonly IContainer _container;

        public LamarCourier_Activity()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddActivity<TestActivity, TestArguments, TestLog>();
                    cfg.AddBus(context => BusControl);
                });
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class LamarCourier_Activity_Endpoint :
        Courier_Activity_Endpoint
    {
        readonly IContainer _container;

        public LamarCourier_Activity_Endpoint()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddActivity<TestActivity, TestArguments, TestLog>()
                        .Endpoints(e => e.Name = "custom-testactivity-execute", e => e.Name = "custom-testactivity-compensate");

                    cfg.AddBus(context => BusControl);
                });
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }
}
