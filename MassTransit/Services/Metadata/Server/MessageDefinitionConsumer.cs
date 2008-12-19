namespace MassTransit.Services.Metadata.Server
{
    using Domain;
    using Messages;

    public class MessageDefinitionConsumer :
        Consumes<MessageDefinition>.All
    {
        private readonly IMetadataRepository _repo;

        public MessageDefinitionConsumer(IMetadataRepository repo)
        {
            _repo = repo;
        }

        public void Consume(MessageDefinition message)
        {
            MessageMetadata data = MessageMetadata.FromDefinition(message);

            _repo.Add(data);
        }
    }
}