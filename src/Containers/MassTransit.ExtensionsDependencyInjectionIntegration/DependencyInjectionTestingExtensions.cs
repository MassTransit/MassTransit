namespace MassTransit.Testing
{
    using System;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;


    public static class DependencyInjectionTestingExtensions
    {
        public static IServiceCollection AddInMemoryTestHarness(this IServiceCollection services,
            Action<IServiceCollectionBusConfigurator> configure = null)
        {
            services.AddSingleton(provider =>
            {
                var testHarness = new InMemoryTestHarness();

                var busRegistrationContext = provider.GetRequiredService<IBusRegistrationContext>();
                testHarness.OnConfigureInMemoryBus += configurator => configurator.ConfigureEndpoints(busRegistrationContext);

                return testHarness;
            });

            services.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.AddBus(context => context.GetRequiredService<InMemoryTestHarness>().BusControl);
            });

            return services;
        }
    }
}
