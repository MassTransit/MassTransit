namespace AuditAzureTableWithMessageTypeFilter
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Azure.Cosmos.Table;
    using System.Collections.Generic;
    
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
                    cfg.UseAzureTableAuditStore(storageAccount, auditTableName, filter => filter.Exclude(typeof(LargeMessage), typeof(SecretMessage)));
                }));
            });
        }
    }

    internal class SecretMessage
    {
        public string TopSecretData { get; set; }
    }

    internal class LargeMessage
    {
        public IEnumerable<string> HugeArray { get; set; }
    }
}