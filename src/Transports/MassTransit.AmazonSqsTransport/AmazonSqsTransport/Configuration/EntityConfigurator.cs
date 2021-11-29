namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;


    public abstract class EntityConfigurator
    {
        protected EntityConfigurator(string entityName, bool durable = true, bool autoDelete = false)
        {
            EntityName = entityName;
            Durable = durable;
            AutoDelete = autoDelete;
        }

        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public string EntityName { get; set; }

        protected abstract AmazonSqsEndpointAddress.AddressType AddressType { get; }

        public virtual AmazonSqsEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return new AmazonSqsEndpointAddress(hostAddress, EntityName, Durable, AutoDelete, AddressType);
        }
    }
}
