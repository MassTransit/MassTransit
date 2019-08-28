namespace MassTransit.Events
{
    using System;


    public class ReceiveEndpointStoppingEvent :
        ReceiveEndpointStopping
    {
        public ReceiveEndpointStoppingEvent(Uri inputAddress, IReceiveEndpoint receiveEndpoint)
        {
            InputAddress = inputAddress;
            ReceiveEndpoint = receiveEndpoint;
        }

        public Uri InputAddress { get; }

        public IReceiveEndpoint ReceiveEndpoint { get; }
    }
}