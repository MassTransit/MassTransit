namespace MassTransit.PublishPipeSpecifications
{
    public interface IPublishPipeSpecificationObserver
    {
        void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class;
    }
}
