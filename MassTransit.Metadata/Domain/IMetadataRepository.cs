namespace MassTransit.Metadata.Domain
{
    using System.Collections.Generic;
    using Messages;

    public interface IMetadataRepository
    {
        void Register(MessageModel data);

        IList<TheMeta> List();
        TheMeta Get(object id);
        void Update(TheMeta data);
    }
}