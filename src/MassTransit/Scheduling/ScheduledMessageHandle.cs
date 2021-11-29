namespace MassTransit.Scheduling
{
    using System;


    public class ScheduledMessageHandle<T> :
        ScheduledMessage<T>
        where T : class
    {
        public ScheduledMessageHandle(Guid tokenId, DateTime scheduledTime, Uri destination, T payload)
        {
            TokenId = tokenId;
            ScheduledTime = scheduledTime;
            Destination = destination;
            Payload = payload;
        }

        public Guid TokenId { get; }
        public DateTime ScheduledTime { get; }
        public Uri Destination { get; }
        public T Payload { get; }
    }
}
