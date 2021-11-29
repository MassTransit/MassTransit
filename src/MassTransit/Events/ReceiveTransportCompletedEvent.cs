namespace MassTransit.Events
{
    using System;
    using Transports;


    public class ReceiveTransportCompletedEvent :
        ReceiveTransportCompleted
    {
        public ReceiveTransportCompletedEvent(Uri inputAddress, DeliveryMetrics metrics)
        {
            InputAddress = inputAddress;
            DeliveryCount = metrics.DeliveryCount;
            ConcurrentDeliveryCount = metrics.ConcurrentDeliveryCount;
        }

        public Uri InputAddress { get; }
        public long DeliveryCount { get; }
        public long ConcurrentDeliveryCount { get; }
    }
}
