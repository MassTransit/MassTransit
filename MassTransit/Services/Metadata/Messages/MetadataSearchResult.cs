namespace MassTransit.Services.Metadata.Messages
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MetadataSearchResult
    {
        public MetadataSearchResult()
        {
            Hits = 0;
            Results = new List<object>();
        }

        public int Hits { get; set; }
        public IList<object> Results { get; set; }
    }
}