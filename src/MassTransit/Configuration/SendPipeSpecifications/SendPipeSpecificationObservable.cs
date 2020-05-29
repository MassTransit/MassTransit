namespace MassTransit.SendPipeSpecifications
{
    using GreenPipes.Util;


    public class SendPipeSpecificationObservable :
        Connectable<ISendPipeSpecificationObserver>,
        ISendPipeSpecificationObserver
    {
        public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
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
