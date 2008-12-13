namespace MassTransit.Metadata.Messages
{
    using System;

    [Serializable]
    public class EndpointModel
    {
        public Uri Address { get; set; }
        public string MachineName { get; set; }
    }
}