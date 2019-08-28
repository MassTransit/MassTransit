namespace MassTransit.Events
{
    using System;


    public class ReceiveEndpointReadyEvent :
        ReceiveEndpointReady
    {
        public ReceiveEndpointReadyEvent(Uri inputAddress, IReceiveEndpoint receiveEndpoint)
        {
            InputAddress = inputAddress;
            ReceiveEndpoint = receiveEndpoint;
        }

        public Uri InputAddress { get; }

        public IReceiveEndpoint ReceiveEndpoint { get; }

    }
}
