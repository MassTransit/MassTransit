namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.ForkJoint.Tests;
    using TestFramework.Futures.Tests;


    class EntityFrameworkFutureTestFixtureConfigurator :
        IFutureTestFixtureConfigurator
    {
        public void ConfigureFutureSagaRepository(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaRepository<FutureState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    r.LockStatementProvider = new SqlServerLockStatementProvider();

                    r.ExistingDbContext<FutureSagaDbContext>();
                });
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddDbContext<FutureSagaDbContext>(builder =>
            {
                FutureSagaDbContextFactory.Apply(builder);
            });
        }

        public async Task OneTimeSetup(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            await using var context = scope.ServiceProvider.GetRequiredService<FutureSagaDbContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public async Task OneTimeTearDown(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            await using var context = scope.ServiceProvider.GetRequiredService<FutureSagaDbContext>();

            await context.Database.EnsureDeletedAsync();
        }
    }


    [TestFixture]
    public class EntityFrameworkFryFutureSpecs :
        FryFuture_Specs
    {
        public EntityFrameworkFryFutureSpecs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkShakeFutureSpecs :
        ShakeFuture_Specs
    {
        public EntityFrameworkShakeFutureSpecs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkFryShakeFutureSpecs :
        FryShakeFuture_Specs
    {
        public EntityFrameworkFryShakeFutureSpecs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkBurgerFutureSpecs :
        BurgerFuture_Specs
    {
        public EntityFrameworkBurgerFutureSpecs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkCalculateFutureSpecs :
        CalculateFuture_Specs
    {
        public EntityFrameworkCalculateFutureSpecs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkOrderFutureSpecs :
        OrderFuture_Specs
    {
        public EntityFrameworkOrderFutureSpecs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkComboFutureSpecs :
        ComboFuture_Specs
    {
        public EntityFrameworkComboFutureSpecs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkPriceCalculationFuture_Specs :
        PriceCalculationFuture_Specs
    {
        public EntityFrameworkPriceCalculationFuture_Specs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkPriceCalculationFuture_RegistrationSpecs :
        PriceCalculationFuture_RegistrationSpecs
    {
        public EntityFrameworkPriceCalculationFuture_RegistrationSpecs()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class EntityFrameworkPriceCalculationFuture_Faulted :
        PriceCalculationFuture_Faulted
    {
        public EntityFrameworkPriceCalculationFuture_Faulted()
            : base(new EntityFrameworkFutureTestFixtureConfigurator())
        {
        }
    }
}
