namespace MassTransit.Services.Metadata.Messages
{
    using System;

    [Serializable]
    public class MetadataSearch
    {
        public string SearchString { get; set; }
    }
}