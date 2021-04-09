using System;
using MassTransit.Context;

namespace MassTransit.EventStoreDbIntegration.Serializers
{
    public interface IMetadataDeserializer
    {
        IHeaderProvider Deserialize(ReadOnlyMemory<byte> headers);
    }
}
