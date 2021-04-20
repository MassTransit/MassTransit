namespace MassTransit.GrpcTransport.Configuration
{
    using System;


    public class GrpcServerConfiguration
    {
        public GrpcServerConfiguration(Uri address)
        {
            Address = address;
        }

        public Uri Address { get; }
    }
}