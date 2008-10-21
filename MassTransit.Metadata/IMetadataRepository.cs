namespace MassTransit.Metadata
{
    using System.Collections.Generic;
    using Domain;

    public interface IMetadataRepository
    {
        IList<MessageMetadata> List();
        void Register(MessageMetadata data);
        MessageMetadata Get(object id);
    }
}