namespace MassTransit.Containers.Tests
{
    using System;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using TestFramework;


    public class AutofacTestFixtureContainerFactory :
        ITestFixtureContainerFactory
    {
        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider BuildServiceProvider(IServiceCollection collection)
        {
            var factory = new AutofacServiceProviderFactory();
            var container = factory.CreateBuilder(collection);

            return factory.CreateServiceProvider(container);
        }
    }
}
