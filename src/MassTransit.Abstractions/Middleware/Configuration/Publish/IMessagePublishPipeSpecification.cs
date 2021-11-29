namespace MassTransit.Configuration
{
    public interface IMessagePublishPipeSpecification<TMessage> :
        IPipeConfigurator<PublishContext<TMessage>>,
        ISpecificationPipeSpecification<PublishContext<TMessage>>
        where TMessage : class
    {
        void AddParentMessageSpecification(ISpecificationPipeSpecification<PublishContext<TMessage>> implementedMessageTypeSpecification);

        /// <summary>
        /// Build the pipe for the specification
        /// </summary>
        /// <returns></returns>
        IPipe<PublishContext<TMessage>> BuildMessagePipe();
    }


    public interface IMessagePublishPipeSpecification :
        IPipeConfigurator<PublishContext>,
        ISpecification
    {
        IMessagePublishPipeSpecification<T> GetMessageSpecification<T>()
            where T : class;
    }
}
