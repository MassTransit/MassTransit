namespace MassTransit.TestComponents
{
    using System;
    using System.Threading.Tasks;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;


    public interface IFutureTestFixtureConfigurator
    {
        void ConfigureFutureSagaRepository(IServiceCollectionBusConfigurator configurator);
        void ConfigureServices(IServiceCollection collection);
        Task OneTimeSetup(IServiceProvider provider);
        Task OneTimeTearDown(IServiceProvider provider);
    }
}
