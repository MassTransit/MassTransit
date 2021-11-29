namespace MassTransit.Containers.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using TestFramework;


    public class SimpleInjectorTestFixtureContainerFactory :
        ITestFixtureContainerFactory
    {
        readonly Container _container;

        public SimpleInjectorTestFixtureContainerFactory()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            _container.Options.EnableAutoVerification = false;
            _container.Options.ResolveUnregisteredConcreteTypes = true;
        }

        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider BuildServiceProvider(IServiceCollection collection)
        {
            return collection.AddSimpleInjector(_container)
                .BuildServiceProvider(true)
                .UseSimpleInjector(_container);
        }
    }
}
