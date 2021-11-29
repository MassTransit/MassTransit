namespace MassTransit.TestFramework
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    public interface IFutureTestFixtureConfigurator
    {
        void ConfigureFutureSagaRepository(IBusRegistrationConfigurator configurator);
        void ConfigureServices(IServiceCollection collection);
        Task OneTimeSetup(IServiceProvider provider);
        Task OneTimeTearDown(IServiceProvider provider);
    }
}
