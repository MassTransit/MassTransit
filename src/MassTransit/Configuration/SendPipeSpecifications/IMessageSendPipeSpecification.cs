namespace MassTransit.SendPipeSpecifications
{
    using GreenPipes;
    using PipeBuilders;


    public interface IMessageSendPipeSpecification<TMessage> :
        IMessageSendPipeConfigurator<TMessage>,
        ISpecificationPipeSpecification<SendContext<TMessage>>
        where TMessage : class
    {
        void AddParentMessageSpecification(ISpecificationPipeSpecification<SendContext<TMessage>> parentSpecification);

        /// <summary>
        /// Build the pipe for the specification
        /// </summary>
        /// <returns></returns>
        IPipe<SendContext<TMessage>> BuildMessagePipe();
    }


    public interface IMessageSendPipeSpecification :
        IPipeConfigurator<SendContext>,
        ISpecification
    {
        IMessageSendPipeSpecification<T> GetMessageSpecification<T>()
            where T : class;
    }
}
