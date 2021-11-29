namespace MassTransit.ActiveMqTransport.Configuration
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
        public string EntityName { get; }

        protected abstract ActiveMqEndpointAddress.AddressType AddressType { get; }

        public ActiveMqEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return new ActiveMqEndpointAddress(hostAddress, EntityName, Durable, AutoDelete, AddressType);
        }
    }
}
