namespace AuditAzureTableWithStorageAccount
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Azure.Cosmos.Table;
    
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("INSERT STORAGE ACCOUNT CONNECTION STRING");
            string auditTableName = "messageaudittable";

            services.AddMassTransit(x =>
            {
                x.AddBus(bus => Bus.Factory.CreateUsingInMemory(cfg =>
                {
                    cfg.UseAzureTableAuditStore(storageAccount, auditTableName);
                }));
            });
        }
    }
}