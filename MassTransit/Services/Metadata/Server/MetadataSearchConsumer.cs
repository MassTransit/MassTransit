namespace MassTransit.Services.Metadata.Server
{
    using Domain;
    using Messages;

    public class MetadataSearchConsumer :
        Consumes<MetadataSearch>.All
    {
        private IServiceBus _bus;
        private IMetadataRepository _repo;

        public MetadataSearchConsumer(IServiceBus bus, IMetadataRepository repo)
        {
            _bus = bus;
            _repo = repo;
        }

        public void Consume(MetadataSearch message)
        {
            _repo.Search(message.SearchString);
            _bus.Publish(new MetadataSearchResult());
        }
    }
}