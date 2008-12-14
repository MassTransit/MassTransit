namespace MassTransit.Metadata.Messages
{
    using System;

    [Serializable]
    public class Metadata
    {
        public Metadata(MessageModel metadata)
        {
            Message = metadata;
        }

        public MessageModel Message { get; set; }
    }
}