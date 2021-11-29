namespace MassTransit.TestFramework
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface ITestFixtureContainerFactory
    {
        IServiceCollection CreateServiceCollection();
        IServiceProvider BuildServiceProvider(IServiceCollection collection);
    }
}
