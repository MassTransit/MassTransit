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

        public void Method4()
        {
        }

        public void Method5()
        {
        }

        public void Method6()
        {
        }
    }
}
