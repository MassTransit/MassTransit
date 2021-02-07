namespace MassTransit
{
    using System.Collections.Generic;
    using Conductor;
    using Conductor.Directory;
    using Conductor.Orchestration;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;


    public static class ConductorRegistrationExtensions
    {
        public static IServiceCollection AddMassTransitServiceDirectory(this IServiceCollection collection)
        {
            collection.AddSingleton(provider => Bind<IBus>.Create(new ServiceDirectoryConfigurator()));
            collection.AddSingleton<IServiceDirectoryConfigurator>(provider => provider.GetRequiredService<Bind<IBus, ServiceDirectoryConfigurator>>().Value);
            collection.AddSingleton(provider =>
            {
                var registry = provider.GetRequiredService<Bind<IBus, ServiceDirectoryConfigurator>>().Value;

                IEnumerable<IConfigureServiceDirectory> configurations = provider.GetServices<IConfigureServiceDirectory>();
                foreach (var configuration in configurations)
                    configuration.Configure(registry);

                return registry.CreateServiceDirectory();
            });
            collection.AddTransient<IOrchestrationProvider, OrchestrationProvider>();

            return collection;
        }
    }
}
