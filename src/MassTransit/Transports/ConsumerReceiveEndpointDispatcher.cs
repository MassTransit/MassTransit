namespace MassTransit.Transports
{
    public class ConsumerReceiveEndpointDispatcher<T> :
        ITypeReceiveEndpointDispatcherFactory
        where T : class, IConsumer
    {
        public IReceiveEndpointDispatcher Create(IReceiveEndpointDispatcherFactory factory, IEndpointNameFormatter formatter)
        {
            var queueName = formatter.Consumer<T>();

            return factory.CreateConsumerReceiver<T>(queueName);
        }
    }
}
