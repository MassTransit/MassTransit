using System;
using MassTransit.Context;

namespace MassTransit.EventStoreDbIntegration.Serializers
{
    public interface IHeadersDeserializer
    {
        IHeaderProvider Deserialize(byte[] headers);
    }
}
