namespace MassTransit.Transformation.TransformConfigurators
{
    using ConsumeConfigurators;
    using MessageData;
    using PipeConfigurators;


    public class MessageDataConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly IMessageDataRepository _repository;

        public MessageDataConfigurationObserver(IConsumePipeConfigurator configurator, IMessageDataRepository repository)
            : base(configurator)
        {
            _repository = repository;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new MessageDataTransformSpecification<TMessage>(_repository);

            configurator.AddPipeSpecification(specification);
        }
    }
}
