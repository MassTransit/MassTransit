namespace MassTransit.Containers.Tests
{
    using System;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using TestFramework;


    public class LamarTestFixtureContainerFactory :
        ITestFixtureContainerFactory
    {
        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider BuildServiceProvider(IServiceCollection collection)
        {
            var factory = new LamarServiceProviderFactory();
            var registry = factory.CreateBuilder(collection);

            return factory.CreateServiceProvider(registry);
        }
    }
}
