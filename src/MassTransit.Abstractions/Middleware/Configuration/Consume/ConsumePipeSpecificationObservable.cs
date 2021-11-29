namespace MassTransit.Configuration
{
    using Util;


    public class ConsumePipeSpecificationObservable :
        Connectable<IConsumePipeSpecificationObserver>,
        IConsumePipeSpecificationObserver
    {
        public void MessageSpecificationCreated<T>(IMessageConsumePipeSpecification<T> specification)
            where T : class
        {
            ForEach(observer => observer.MessageSpecificationCreated(specification));
        }
    }
}
