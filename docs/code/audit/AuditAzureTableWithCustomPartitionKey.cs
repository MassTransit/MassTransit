namespace AuditAzureTableWithCustomPartitionKey
{
    using MassTransit;
    using MassTransit.AzureTable;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.DependencyInjection;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("INSERT STORAGE ACCOUNT CONNECTION STRING");
            string auditTableName = "messageaudittable";
            string PartitionKey = "CustomPartitionKey";

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseAzureTableAuditStore(storageAccount, auditTableName, new ConstantPartitionKeyFormatter(PartitionKey));
                });
            });
        }
    }

    class ConstantPartitionKeyFormatter :
        IPartitionKeyFormatter
    {
        readonly string _partitionKey;

        public ConstantPartitionKeyFormatter(string partitionKey)
        {
            _partitionKey = partitionKey;
        }

        public string Format<T>(AuditRecord record)
            where T : class
        {
            return _partitionKey;
        }
    }
}
