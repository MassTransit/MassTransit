namespace MassTransit.Containers.Tests
{
    using System;
    using Castle.Windsor.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using TestFramework;


    public class CastleWindsorTestFixtureContainerFactory :
        ITestFixtureContainerFactory
    {
        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider BuildServiceProvider(IServiceCollection collection)
        {
            var factory = new WindsorServiceProviderFactory();
            var container = factory.CreateBuilder(collection);

            return factory.CreateServiceProvider(container);
        }
    }
}
