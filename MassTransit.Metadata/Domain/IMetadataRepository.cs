namespace MassTransit.Metadata.Domain
{
    using System.Collections.Generic;
    using Services.Metadata.Messages;

    public interface IMetadataRepository
    {
        void Register(MessageDefinition data);

        IList<MessageMetadata> List();
        MessageMetadata Get(object id);
        void Update(MessageMetadata data);
    }
}