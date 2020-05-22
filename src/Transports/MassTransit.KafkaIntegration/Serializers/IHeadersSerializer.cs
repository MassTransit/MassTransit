namespace MassTransit.KafkaIntegration.Serializers
{
    using System.Collections.Generic;
    using Confluent.Kafka;


    public interface IHeadersSerializer
    {
        Headers Serialize(Dictionary<string, object> headers);
    }
}
