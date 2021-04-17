using System;
using EventStore.Client;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public class HostSettings :
        IHostSettings
    {
        public string ConnectionString { get; set; }
        public string ConnectionName { get; set; }
        public UserCredentials DefaultCredentials { get; set; }
    }
}
