using Azure.Data.Tables;

using Azure.Data.Tables;

namespace MassTransit.Azure.Table.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.ForkJoint.Tests;
    using TestFramework.Futures.Tests;


    class AzureTableFutureTestFixtureConfigurator :
        IFutureTestFixtureConfigurator
    {
        const string TableName = "futurestate";

        public void ConfigureFutureSagaRepository(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaRepository<FutureState>()
                .AzureTableRepository(r =>
                {
                    r.ConnectionFactory(provider => provider.GetRequiredService<TableServiceClient>().GetTableClient(TableName));
                });
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton(provider =>
                {
                    var connectionString = Configuration.StorageAccount;
                    return new TableServiceClient(connectionString);
                })
                .AddSingleton(provider =>
                {
                    var storageAccount = provider.GetRequiredService<TableServiceClient>();
                    var tableClient = storageAccount.CreateTableIfNotExists(TableName);
                    return storageAccount;
                });
        }

        public async Task OneTimeSetup(IServiceProvider provider)
        {
            var table = provider.GetRequiredService<TableServiceClient>().GetTableClient(TableName);

            await table.CreateIfNotExistsAsync();

            await foreach (var page in table.QueryAsync<TableEntity>().AsPages())
            {
                foreach (var group in page.Values.GroupBy(x => x.PartitionKey))
                {
                    var batchDeleteOperations = new List<TableTransactionAction>();

                    foreach (var entity in group)
                    {
                        batchDeleteOperations.Add(new TableTransactionAction(TableTransactionActionType.Delete, entity));
                    }

                    await table.SubmitTransactionAsync(batchDeleteOperations);
                }
            }
        }

        public Task OneTimeTearDown(IServiceProvider provider)
        {
            return Task.CompletedTask;
        }
    }


    [TestFixture]
    public class AzureTableFryFutureSpecs :
        FryFuture_Specs
    {
        public AzureTableFryFutureSpecs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTableShakeFutureSpecs :
        ShakeFuture_Specs
    {
        public AzureTableShakeFutureSpecs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTableFryShakeFutureSpecs :
        FryShakeFuture_Specs
    {
        public AzureTableFryShakeFutureSpecs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTableBurgerFutureSpecs :
        BurgerFuture_Specs
    {
        public AzureTableBurgerFutureSpecs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTableCalculateFutureSpecs :
        CalculateFuture_Specs
    {
        public AzureTableCalculateFutureSpecs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTableOrderFutureSpecs :
        OrderFuture_Specs
    {
        public AzureTableOrderFutureSpecs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTableComboFutureSpecs :
        ComboFuture_Specs
    {
        public AzureTableComboFutureSpecs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTablePriceCalculationFuture_Specs :
        PriceCalculationFuture_Specs
    {
        public AzureTablePriceCalculationFuture_Specs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTablePriceCalculationFuture_RegistrationSpecs :
        PriceCalculationFuture_RegistrationSpecs
    {
        public AzureTablePriceCalculationFuture_RegistrationSpecs()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureTablePriceCalculationFuture_Faulted :
        PriceCalculationFuture_Faulted
    {
        public AzureTablePriceCalculationFuture_Faulted()
            : base(new AzureTableFutureTestFixtureConfigurator())
        {
        }
    }
}
