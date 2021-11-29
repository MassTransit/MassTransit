namespace MassTransit.Configuration
{
    public interface IMessageConsumePipeSpecification<TMessage> :
        IMessageConsumePipeConfigurator<TMessage>,
        ISpecificationPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        void AddParentMessageSpecification(ISpecificationPipeSpecification<ConsumeContext<TMessage>> parentSpecification);

        IPipe<ConsumeContext<TMessage>> BuildMessagePipe(IPipe<ConsumeContext<TMessage>> pipe);
    }


    public interface IMessageConsumePipeSpecification :
        IPipeConfigurator<ConsumeContext>,
        ISpecification
    {
        IMessageConsumePipeSpecification<T> GetMessageSpecification<T>()
            where T : class;
    }
}
