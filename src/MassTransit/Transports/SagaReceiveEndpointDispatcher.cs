namespace MassTransit.Transports
{
    public class SagaReceiveEndpointDispatcher<T> :
        ITypeReceiveEndpointDispatcherFactory
        where T : class, ISaga
    {
        public IReceiveEndpointDispatcher Create(IReceiveEndpointDispatcherFactory factory, IEndpointNameFormatter formatter)
        {
            var queueName = formatter.Saga<T>();

            return factory.CreateSagaReceiver<T>(queueName);
        }
    }
}
