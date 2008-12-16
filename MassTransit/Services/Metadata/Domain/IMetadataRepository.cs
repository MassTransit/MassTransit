namespace MassTransit.Services.Metadata.Domain
{
    using System.Collections.Generic;
    using Messages;

    //hides the persistance and indexing
    public interface IMetadataRepository
    {
        void Register(MessageDefinition data);

        IList<MessageMetadata> List();
        MessageMetadata Get(object id);
        void Update(MessageMetadata data);

        IList<MessageMetadata> Search(string searchString);
    }
}