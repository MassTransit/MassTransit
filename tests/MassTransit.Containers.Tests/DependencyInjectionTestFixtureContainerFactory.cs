namespace MassTransit.Containers.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using TestFramework;


    public class DependencyInjectionTestFixtureContainerFactory :
        ITestFixtureContainerFactory
    {
        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider BuildServiceProvider(IServiceCollection collection)
        {
            return collection.BuildServiceProvider(true);
        }
    }
}
