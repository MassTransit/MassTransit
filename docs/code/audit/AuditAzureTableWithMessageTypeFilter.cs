namespace AuditAzureTableWithMessageTypeFilter
{
    using System.Collections.Generic;
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
                    cfg.UseAzureTableAuditStore(storageAccount, auditTableName, filter => filter.Exclude(typeof(LargeMessage), typeof(SecretMessage)));
                });
            });
        }
    }

    class SecretMessage
    {
        public string TopSecretData { get; set; }
    }

    class LargeMessage
    {
        public IEnumerable<string> HugeArray { get; set; }
    }
}
