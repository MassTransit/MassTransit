namespace MassTransit.Events
{
    using System;


    public class ReceiveEndpointCompletedEvent :
        ReceiveEndpointCompleted
    {
        readonly ReceiveTransportCompleted _completed;

        public ReceiveEndpointCompletedEvent(ReceiveTransportCompleted completed, IReceiveEndpoint receiveEndpoint)
        {
            _completed = completed;
            ReceiveEndpoint = receiveEndpoint;
        }

        public Uri InputAddress => _completed.InputAddress;
        public long DeliveryCount => _completed.DeliveryCount;
        public long ConcurrentDeliveryCount => _completed.ConcurrentDeliveryCount;

        public IReceiveEndpoint ReceiveEndpoint { get; }
    }
}
