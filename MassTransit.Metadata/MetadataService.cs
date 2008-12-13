namespace MassTransit.Metadata
{
    using Domain;
    using Messages;
    

    public class MetadataService :
        Consumes<MetadataMessage>.All
    {
        private readonly IMetadataRepository _repository;

        public MetadataService(IMetadataRepository repository)
        {
            _repository = repository;
        }

        public void Consume(MetadataMessage message)
        {
            _repository.Register(message.Metadata);
        }
    }
}