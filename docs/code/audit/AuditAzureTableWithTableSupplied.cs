namespace AuditAzureTableWithTableSupplied
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Azure.Cosmos.Table;
    
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("INSERT STORAGE ACCOUNT CONNECTION STRING");
            CloudTableClient client = storageAccount.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("audittablename");
            table.CreateIfNotExists();

            services.AddMassTransit(x =>
            {
                x.AddBus(bus => Bus.Factory.CreateUsingInMemory(cfg =>
                {
                    cfg.UseAzureTableAuditStore(table);
                }));
            });
        }
    }
}