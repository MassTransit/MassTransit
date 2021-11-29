namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.ForkJoint.Tests;
    using TestFramework.Futures.Tests;


    class InMemoryFutureTestFixtureConfigurator :
        IFutureTestFixtureConfigurator
    {
        public void ConfigureFutureSagaRepository(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaRepository<FutureState>()
                .InMemoryRepository();
        }

        public void ConfigureServices(IServiceCollection collection)
        {
        }

        public Task OneTimeSetup(IServiceProvider provider)
        {
            return Task.CompletedTask;
        }

        public Task OneTimeTearDown(IServiceProvider provider)
        {
            return Task.CompletedTask;
        }
    }


    [TestFixture]
    public class InMemoryFryFutureSpecs :
        FryFuture_Specs
    {
        public InMemoryFryFutureSpecs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryShakeFutureSpecs :
        ShakeFuture_Specs
    {
        public InMemoryShakeFutureSpecs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryFryShakeFutureSpecs :
        FryShakeFuture_Specs
    {
        public InMemoryFryShakeFutureSpecs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryBurgerFutureSpecs :
        BurgerFuture_Specs
    {
        public InMemoryBurgerFutureSpecs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryCalculateFutureSpecs :
        CalculateFuture_Specs
    {
        public InMemoryCalculateFutureSpecs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryOrderFutureSpecs :
        OrderFuture_Specs
    {
        public InMemoryOrderFutureSpecs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryComboFutureSpecs :
        ComboFuture_Specs
    {
        public InMemoryComboFutureSpecs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryPriceCalculationFuture_Specs :
        PriceCalculationFuture_Specs
    {
        public InMemoryPriceCalculationFuture_Specs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryPriceCalculationFuture_RegistrationSpecs :
        PriceCalculationFuture_RegistrationSpecs
    {
        public InMemoryPriceCalculationFuture_RegistrationSpecs()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class InMemoryPriceCalculationFuture_Faulted :
        PriceCalculationFuture_Faulted
    {
        public InMemoryPriceCalculationFuture_Faulted()
            : base(new InMemoryFutureTestFixtureConfigurator())
        {
        }
    }
}
