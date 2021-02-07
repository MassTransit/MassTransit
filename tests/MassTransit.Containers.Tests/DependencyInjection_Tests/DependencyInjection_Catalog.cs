namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Conductor;
    using Conductor.Directory;
    using ExtensionsDependencyInjectionIntegration;
    using Futures;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestComponents.ForkJoint.Contracts;
    using TestComponents.ForkJoint.ItineraryPlanners;
    using TestComponents.ForkJoint.Services;


    [TestFixture]
    public class DependencyInjection_Catalog :
        Common_Catalog
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Catalog()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .AddMassTransitServiceDirectory()
                .AddGenericRequestClient()
                .AddSingleton<IFryer, Fryer>()
                .AddSingleton<IGrill, Grill>()
                .AddSingleton<IShakeMachine, ShakeMachine>()
                .AddScoped<IItineraryPlanner<OrderBurger>, BurgerItineraryPlanner>()
                .BuildServiceProvider(true);
        }

        protected override IServiceDirectory Directory => _provider.GetRequiredService<IServiceDirectory>();
        protected override IOrchestrationProvider ExecutorFactory => _provider.GetRequiredService<IOrchestrationProvider>();
        protected override ServiceDirectoryConfigurator ServiceDirectoryConfigurator => _provider.GetRequiredService<Bind<IBus, ServiceDirectoryConfigurator>>().Value;

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();

        [OneTimeSetUp]
        public void ResolveTheBus()
        {
            _provider.GetRequiredService<IBus>();
        }

        protected override IRequestClient<T> GetRequestClient<T>()
        {
            var scope = _provider.CreateScope();

            return scope.ServiceProvider.GetRequiredService<IRequestClient<T>>();
        }
    }
}
