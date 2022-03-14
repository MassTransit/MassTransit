namespace AuditAzureTableWithStorageAccount
{
    using MassTransit;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.DependencyInjection;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("INSERT STORAGE ACCOUNT CONNECTION STRING");
            string auditTableName = "messageaudittable";

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseAzureTableAuditStore(storageAccount, auditTableName);
                });
            });
        }
    }
}
