namespace MassTransit.MongoDbIntegration.Tests.Courier
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Courier.Contracts;
    using Metadata;


    class RoutingSlipActivityCompletedEvent :
        RoutingSlipActivityCompleted
    {
        public RoutingSlipActivityCompletedEvent(Guid trackingNumber, string activityName,
            Guid executionId, DateTime timestamp)
        {
            Timestamp = timestamp;
            TrackingNumber = trackingNumber;
            ActivityName = activityName;
            ExecutionId = executionId;

            Variables = new Dictionary<string, object> { { "Content", "Goodbye, cruel world." } };

            Data = new Dictionary<string, object> { { "OriginalContent", "Hello, World!" } };

            Arguments = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Data { get; private set; }

        public Guid TrackingNumber { get; private set; }

        public DateTime Timestamp { get; private set; }

        public TimeSpan Duration { get; private set; }

        public Guid ExecutionId { get; private set; }
        public string ActivityName { get; private set; }

        public HostInfo Host => HostMetadataCache.Host;

        public IDictionary<string, object> Arguments { get; private set; }

        public IDictionary<string, object> Variables { get; private set; }
    }
}
