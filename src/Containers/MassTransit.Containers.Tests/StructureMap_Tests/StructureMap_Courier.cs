namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using StructureMap;
    using TestFramework.Courier;


    [TestFixture]
    public class StructureMapCourier_ExecuteActivity :
        Courier_ExecuteActivity
    {
        readonly IContainer _container;

        public StructureMapCourier_ExecuteActivity()
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

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }


    [TestFixture]
    public class StructureMapCourier_ExecuteActivity_Endpoint :
        Courier_ExecuteActivity_Endpoint
    {
        readonly IContainer _container;

        public StructureMapCourier_ExecuteActivity_Endpoint()
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

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }


    [TestFixture]
    public class StructureMapCourier_Activity :
        Courier_Activity
    {
        readonly IContainer _container;

        public StructureMapCourier_Activity()
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

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }


    [TestFixture]
    public class StructureMapCourier_Activity_Endpoint :
        Courier_Activity_Endpoint
    {
        readonly IContainer _container;

        public StructureMapCourier_Activity_Endpoint()
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

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }
}
