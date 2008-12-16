namespace MassTransit.Services.Metadata.Domain
{
    using System.Collections.Generic;
    using Messages;

    public interface IMetadataRepository
    {
        void Register(MessageDefinition data);

        IList<MessageMetadata> List();
        MessageMetadata Get(object id);
        void Update(MessageMetadata data);
    }
}