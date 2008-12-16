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
            //transform here first?
            _repo.Register(message);
        }
    }
}