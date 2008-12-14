namespace MassTransit.Metadata.Messages
{
    using System;

    [Serializable]
    public class MetadataSearchResult
    {
        public int Hits { get; set; }
    }
}