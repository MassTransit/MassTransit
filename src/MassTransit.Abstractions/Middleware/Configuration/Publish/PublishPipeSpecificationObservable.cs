namespace MassTransit.Configuration
{
    using Util;


    public class PublishPipeSpecificationObservable :
        Connectable<IPublishPipeSpecificationObserver>,
        IPublishPipeSpecificationObserver
    {
        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class
        {
            ForEach(observer => observer.MessageSpecificationCreated(specification));
        }
    }
}
