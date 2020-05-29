namespace MassTransit.Events
{
    using System;


    public class HostReadyEvent :
        HostReady
    {
        public HostReadyEvent(Uri hostAddress, ReceiveEndpointReady[] receiveEndpoints)
        {
            HostAddress = hostAddress;
            ReceiveEndpoints = receiveEndpoints;
        }

        public Uri HostAddress { get; }

        public ReceiveEndpointReady[] ReceiveEndpoints { get; }
    }
}
