namespace MassTransit.Azure.Table.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
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
                    r.ConnectionFactory(provider => provider.GetRequiredService<CloudTableClient>().GetTableReference(TableName));
                });
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton(provider =>
                {
                    var connectionString = Configuration.StorageAccount;
                    var storageAccount = CloudStorageAccount.Parse(connectionString);

                    return storageAccount;
                })
                .AddSingleton(provider =>
                {
                    var storageAccount = provider.GetRequiredService<CloudStorageAccount>();

                    var tableClient = storageAccount.CreateCloudTableClient();

                    return tableClient;
                });
        }

        public async Task OneTimeSetup(IServiceProvider provider)
        {
            var table = provider.GetRequiredService<CloudTableClient>().GetTableReference(TableName);

            await table.CreateIfNotExistsAsync();

            var query = new TableQuery();
            TableQuerySegment<DynamicTableEntity> segment = await table.ExecuteQuerySegmentedAsync(query, null);

            while (segment.Results.Count > 0)
            {
                foreach (IGrouping<string, DynamicTableEntity> key in segment.Results.GroupBy(x => x.PartitionKey))
                {
                    var batchDeleteOperation = new TableBatchOperation();

                    foreach (var row in key)
                        batchDeleteOperation.Delete(row);

                    await table.ExecuteBatchAsync(batchDeleteOperation);
                }

                segment = await table.ExecuteQuerySegmentedAsync(query, segment.ContinuationToken);
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
