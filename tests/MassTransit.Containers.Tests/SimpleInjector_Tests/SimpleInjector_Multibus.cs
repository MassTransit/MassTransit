namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System.Collections.Generic;
    using AspNetCoreIntegration;
    using Common_Tests;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Registration;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using SimpleInjectorIntegration.Multibus;


    [TestFixture]
    public class SimpleInjector_MultiBus : Common_MultiBus
    {
        readonly Container _container;

        protected override IBusOne One => _container.GetInstance<IBusOne>();

        protected override IEnumerable<IHostedService> HostedServices => _container.GetAllInstances<IHostedService>();

        protected override IRequestClient<T> GetRequestClient<T>()
        {
            return _container.GetInstance<IRequestClient<T>>();
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();

        public SimpleInjector_MultiBus()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.EnableAutoVerification = false;
            container.Options.ResolveUnregisteredConcreteTypes = false;

            container.RegisterInstance<ILoggerFactory>(LoggerFactory);
            container.RegisterSingleton(typeof(ILogger<>), typeof(Logger<>));

            container.AddMassTransit(x =>
            {
                ConfigureRegistration(x);

                x.AddBus(provider => BusControl);
            });

            container.RegisterInstance(Task1);
            container.RegisterInstance(Task2);

            container.AddMassTransit<IBusOne, BusOne>(ConfigureOne);
            container.AddMassTransit<IBusTwo>(ConfigureTwo);

            container.Collection.Append<IHostedService>(() =>
            {
                var busDepot = container.GetInstance<IBusDepot>();
                return new MassTransitHostedService(busDepot, true);
            }, Lifestyle.Singleton);

            _container = container;
        }
    }
}
