namespace MassTransit.Configuration
{
    public interface IConsumePipeSpecificationObserver
    {
        void MessageSpecificationCreated<T>(IMessageConsumePipeSpecification<T> specification)
            where T : class;
    }
}
