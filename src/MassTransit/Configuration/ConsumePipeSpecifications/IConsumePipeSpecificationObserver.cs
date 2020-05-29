namespace MassTransit.ConsumePipeSpecifications
{
    public interface IConsumePipeSpecificationObserver
    {
        void MessageSpecificationCreated<T>(IMessageConsumePipeSpecification<T> specification)
            where T : class;
    }
}
