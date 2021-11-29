namespace MassTransit.Containers.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using StructureMap;
    using TestFramework;


    public class StructureMapTestFixtureContainerFactory :
        ITestFixtureContainerFactory
    {
        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider BuildServiceProvider(IServiceCollection collection)
        {
            var registry = new Registry();

            registry.Populate(collection);

            var factory = new StructureMapServiceProviderFactory(registry);
            return factory.CreateServiceProvider(registry);
        }
    }
}
