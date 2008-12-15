namespace MassTransit.Metadata.Domain
{
    using System.Collections.Generic;
    using Messages;

    public interface IMetadataRepository
    {
        void Register(MessageModel data);

        IList<MessageMetadata> List();
        MessageMetadata Get(object id);
        void Update(MessageMetadata data);
    }
}