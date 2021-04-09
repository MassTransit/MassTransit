using System;
using EventStore.Client;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public class HostSettings :
        IHostSettings
    {
        public Func<IConfigurationServiceProvider, EventStoreClient> ExistingClientFactory { get; set; }
        public string ConnectionString { get; set; }
        public string ConnectionName { get; set; }
        public UserCredentials DefaultCredentials { get; set; }
    }
}
