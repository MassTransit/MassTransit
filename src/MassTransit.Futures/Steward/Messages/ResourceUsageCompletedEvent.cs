namespace MassTransit.Steward.Messages
{
    using System;
    using Contracts.Events;


    class ResourceUsageCompletedEvent :
        ResourceUsageCompleted
    {
        public ResourceUsageCompletedEvent(Uri resource, DateTime timestamp, TimeSpan duration)
            : this(resource, Guid.Empty, timestamp, duration)
        {
        }

        public ResourceUsageCompletedEvent(Uri resource, Guid dispatchId, DateTime timestamp, TimeSpan duration)
        {
            Timestamp = timestamp;
            Duration = duration;
            Resource = resource;
            DispatchId = dispatchId;

            EventId = NewId.NextGuid();
        }

        public Guid EventId { get; private set; }
        public Guid DispatchId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public Uri Resource { get; private set; }
    }
}