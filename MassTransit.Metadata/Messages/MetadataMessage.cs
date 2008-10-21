namespace MassTransit.Metadata.Messages
{
    using System;
    using Domain;

    [Serializable]
    public class MetadataMessage
    {
        private readonly MessageMetadata _metadata;

        public MetadataMessage(MessageMetadata metadata)
        {
            _metadata = metadata;
        }

        public MessageMetadata Metadata
        {
            get { return _metadata; }
        }
    }
}