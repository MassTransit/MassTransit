namespace MassTransit.Configuration
{
    public interface ISendPipeSpecificationObserver
    {
        void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
            where T : class;
    }
}
