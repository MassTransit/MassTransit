namespace MassTransit.PublishPipeSpecifications
{
    using GreenPipes.Util;


    public class PublishPipeSpecificationObservable :
        Connectable<IPublishPipeSpecificationObserver>,
        IPublishPipeSpecificationObserver
    {
        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
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
