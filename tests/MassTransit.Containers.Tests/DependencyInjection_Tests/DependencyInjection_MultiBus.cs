namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using System.Collections.Generic;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MultiBus;
    using NUnit.Framework;
    using TestFramework.Logging;


    [TestFixture]
    public class DependencyInjection_MultiBus :
        Common_MultiBus
    {
        readonly IServiceProvider _provider;

        protected override IBusOne One => _provider.GetRequiredService<IBusOne>();
        protected override HealthCheckService HealthCheckService => _provider.GetRequiredService<HealthCheckService>();
        protected override IEnumerable<IHostedService> HostedServices => _provider.GetServices<IHostedService>();

        public DependencyInjection_MultiBus()
        {
            var services = new ServiceCollection();

            services.TryAddSingleton<ILoggerFactory>(provider => new TestOutputLoggerFactory(true));
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            services.AddSingleton(Task1);
            services.AddSingleton(Task2);

            services.AddMassTransit<IBusOne, BusOne>(ConfigureOne);
            services.AddMassTransit<IBusTwo>(ConfigureTwo);

            services.AddMassTransitHostedService();

            _provider = services.BuildServiceProvider(true);
        }
    }
}
