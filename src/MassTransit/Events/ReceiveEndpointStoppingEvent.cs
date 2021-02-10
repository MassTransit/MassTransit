namespace MassTransit.Events
{
    using System;


    public class ReceiveEndpointStoppingEvent :
        ReceiveEndpointStopping
    {
        public ReceiveEndpointStoppingEvent(Uri inputAddress, IReceiveEndpoint receiveEndpoint, bool removed)
        {
            InputAddress = inputAddress;
            ReceiveEndpoint = receiveEndpoint;
            Removed = removed;
        }

        public bool Removed { get; }

        public Uri InputAddress { get; }

        public IReceiveEndpoint ReceiveEndpoint { get; }
    }
}
