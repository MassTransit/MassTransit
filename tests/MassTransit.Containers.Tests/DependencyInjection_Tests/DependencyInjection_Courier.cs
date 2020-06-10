namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Courier;


    [TestFixture]
    public class DependencyInjectionCourier_ExecuteActivity :
        Courier_ExecuteActivity
    {
        readonly IServiceProvider _container;

        public DependencyInjectionCourier_ExecuteActivity()
        {
            var builder = new ServiceCollection();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>();
                cfg.AddBus(context => BusControl);
            });

            _container = builder.BuildServiceProvider(true);
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjectionCourier_ExecuteActivity_Endpoint :
        Courier_ExecuteActivity_Endpoint
    {
        readonly IServiceProvider _container;

        public DependencyInjectionCourier_ExecuteActivity_Endpoint()
        {
            var builder = new ServiceCollection();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddExecuteActivity<SetVariableActivity, SetVariableArguments>()
                    .Endpoint(e => e.Name = "custom-setvariable-execute");

                cfg.AddBus(context => BusControl);
            });

            _container = builder.BuildServiceProvider();
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjectionCourier_Activity :
        Courier_Activity
    {
        readonly IServiceProvider _container;

        public DependencyInjectionCourier_Activity()
        {
            var builder = new ServiceCollection();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>();
                cfg.AddBus(context => BusControl);
            });

            _container = builder.BuildServiceProvider();
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjectionCourier_Activity_Endpoint :
        Courier_Activity_Endpoint
    {
        readonly IServiceProvider _container;

        public DependencyInjectionCourier_Activity_Endpoint()
        {
            var builder = new ServiceCollection();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddActivity<TestActivity, TestArguments, TestLog>()
                    .Endpoints(e => e.Name = "custom-testactivity-execute", e => e.Name = "custom-testactivity-compensate");

                cfg.AddBus(context => BusControl);
            });

            _container = builder.BuildServiceProvider();
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjection_Courier_Activity_Filter :
        Common_Activity_Filter
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Courier_Activity_Filter()
        {
            var services = new ServiceCollection();
            services.AddScoped(_ => new MyId(Guid.NewGuid()));
            services.AddScoped(typeof(ScopedFilter<>));
            services.AddSingleton(ExecuteTaskCompletionSource);

            services.AddMassTransit(ConfigureRegistration);

            _provider = services.BuildServiceProvider();
        }

        protected override void ConfigureFilter(IConsumePipeConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseExecuteActivityFilter(configurator, typeof(ScopedFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }
}
