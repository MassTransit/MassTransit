namespace MassTransit.Metadata.Messages
{
    using System;

    [Serializable]
    public class MetadataMessage
    {
        public MetadataMessage(MessageModel metadata)
        {
            Metadata = metadata;
        }

        public MessageModel Metadata { get; set; }
    }
}