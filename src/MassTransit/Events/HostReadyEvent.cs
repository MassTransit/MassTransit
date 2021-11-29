namespace MassTransit.Events
{
    using System;


    public class HostReadyEvent :
        HostReady
    {
        public HostReadyEvent(Uri hostAddress, ReceiveEndpointReady[] receiveEndpoints, RiderReady[] riders)
        {
            HostAddress = hostAddress;
            ReceiveEndpoints = receiveEndpoints;
            Riders = riders;
        }

        public Uri HostAddress { get; }

        public ReceiveEndpointReady[] ReceiveEndpoints { get; }

        public RiderReady[] Riders { get; }
    }
}
