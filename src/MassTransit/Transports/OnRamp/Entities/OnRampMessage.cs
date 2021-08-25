using System;

namespace MassTransit.Transports.Outbox.Entities
{
    public class OnRampMessage
    {
        public string OnRampName { get; set; } // make this part of composite PK, and it should always be present (kind of think of it as a cluster name)
        public Guid Id { get; set; }

        public string InstanceId { get; set; } // this should be nullable, and not part of PK. It represents the node in cluster that has fetched it for processing
        public int Retries { get; set; }

        public JsonSerializedMessage SerializedMessage { get; set; }

        public DateTime Added { get; set; }
    }
}
