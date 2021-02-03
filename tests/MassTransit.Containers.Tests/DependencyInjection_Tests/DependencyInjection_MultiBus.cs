namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using System.Collections.Generic;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MultiBus;
    using NUnit.Framework;


    [TestFixture]
    public class DependencyInjection_MultiBus :
        Common_MultiBus
    {
        readonly IServiceProvider _provider;

        protected override IBusOne One => _provider.GetRequiredService<IBusOne>();
        protected override IEnumerable<IHostedService> HostedServices => _provider.GetServices<IHostedService>();

        protected override IRequestClient<T> GetRequestClient<T>()
        {
            var scope = _provider.CreateScope();

            return scope.ServiceProvider.GetRequiredService<IRequestClient<T>>();
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();

        public DependencyInjection_MultiBus()
        {
            var services = new ServiceCollection();

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            services.AddMassTransit(x =>
            {
                ConfigureRegistration(x);

                x.AddBus(provider => BusControl);
            });

            services.AddSingleton(Task1);
            services.AddSingleton(Task2);

            services.AddMassTransit<IBusOne, BusOne>(ConfigureOne);
            services.AddMassTransit<IBusTwo>(ConfigureTwo);

            services.AddMassTransitHostedService(true);

            _provider = services.BuildServiceProvider(true);
        }
    }
}
