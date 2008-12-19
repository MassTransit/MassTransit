namespace MassTransit.Services.Metadata.Domain
{
    //hides the persistance and indexing
    public interface IMetadataRepository
    {
        void Add(MessageMetadata metadata);
    }
}