using System;
using EventStore.Client;
using MassTransit.Context;

namespace MassTransit.EventStoreDbIntegration.Serializers
{
    public interface IHeadersDeserializer
    {
        IHeaderProvider Deserialize(ResolvedEvent resolvedEvent);
    }
}
