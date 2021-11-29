namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;


    public abstract class ServiceBusEntityConfigurator :
        IServiceBusEntityConfigurator
    {
        protected ServiceBusEntityConfigurator()
        {
            DefaultMessageTimeToLive = Defaults.DefaultMessageTimeToLive;
        }

        public TimeSpan? AutoDeleteOnIdle { get; set; }

        public TimeSpan? DefaultMessageTimeToLive { get; set; }

        public bool? EnableBatchedOperations { get; set; }

        public string UserMetadata { get; set; }
    }
}
