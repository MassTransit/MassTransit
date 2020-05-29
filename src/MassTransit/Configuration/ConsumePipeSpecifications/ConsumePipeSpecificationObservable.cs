namespace MassTransit.ConsumePipeSpecifications
{
    using GreenPipes.Util;


    public class ConsumePipeSpecificationObservable :
        Connectable<IConsumePipeSpecificationObserver>,
        IConsumePipeSpecificationObserver
    {
        public void MessageSpecificationCreated<T>(IMessageConsumePipeSpecification<T> specification)
            where T : class
        {
            All(observer =>
            {
                observer.MessageSpecificationCreated(specification);

                return true;
            });
        }
    }
}
