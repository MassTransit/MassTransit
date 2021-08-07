namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System.Collections.Generic;
    using AspNetCoreIntegration;
    using Autofac;
    using Common_Tests;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Registration;


    [TestFixture]
    public class Autofac_MultiBus :
        Common_MultiBus
    {
        readonly IContainer _container;

        protected override IBusOne One => _container.Resolve<IBusOne>();
        protected override IEnumerable<IHostedService> HostedServices => _container.Resolve<IEnumerable<IHostedService>>();

        protected override IRequestClient<T> GetRequestClient<T>()
        {
            var scope = _container.BeginLifetimeScope();

            return scope.Resolve<IRequestClient<T>>();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();

        public Autofac_MultiBus()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(LoggerFactory).As<ILoggerFactory>();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            builder.AddMassTransit(x =>
            {
                ConfigureRegistration(x);

                x.AddBus(provider => BusControl);
            });

            builder.RegisterInstance(Task1);
            builder.RegisterInstance(Task2);

            builder.AddMassTransit<IBusOne, BusOne>(ConfigureOne);
            builder.AddMassTransit<IBusTwo>(ConfigureTwo);

            builder.Register<IHostedService>(context =>
            {
                var busDepot = context.Resolve<IBusDepot>();
                return new MassTransitHostedService(busDepot, true);
            }).SingleInstance();

            _container = builder.Build();
        }
    }
}
