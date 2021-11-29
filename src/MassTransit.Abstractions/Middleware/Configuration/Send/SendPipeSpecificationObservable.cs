namespace MassTransit.Configuration
{
    using Util;


    public class SendPipeSpecificationObservable :
        Connectable<ISendPipeSpecificationObserver>,
        ISendPipeSpecificationObserver
    {
        public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
            where T : class
        {
            ForEach(observer => observer.MessageSpecificationCreated(specification));
        }
    }
}
