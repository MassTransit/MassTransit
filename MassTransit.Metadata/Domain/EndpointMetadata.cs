namespace MassTransit.Metadata.Domain
{
    using System;

    public class EndpointMetadata
    {
        public Uri Address { get; set; }
        public string Description { get; set; }
    }
}