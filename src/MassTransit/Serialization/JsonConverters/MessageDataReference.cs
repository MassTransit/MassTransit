namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using Newtonsoft.Json;


    public class MessageDataReference
    {
        [JsonProperty("data-ref")]
        public Uri Reference { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("data")]
        public byte[] Data { get; set; }
    }
}
