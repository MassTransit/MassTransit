namespace MassTransit.Events
{
    using System;


    public class ReceiveEndpointReadyEvent :
        ReceiveEndpointReady
    {
        public ReceiveEndpointReadyEvent(Uri inputAddress)
        {
            InputAddress = inputAddress;
        }

        public Uri InputAddress { get; }
    }
}