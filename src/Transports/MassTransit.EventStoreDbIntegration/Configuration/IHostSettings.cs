using System;
using EventStore.Client;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IHostSettings
    {
        bool UseExistingClient => ExistingClientFactory != null;
        Func<IConfigurationServiceProvider, EventStoreClient> ExistingClientFactory { get; }
        string ConnectionString { get; }
        string ConnectionName { get; }
        UserCredentials DefaultCredentials { get; }
    }
}
