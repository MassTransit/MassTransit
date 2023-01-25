namespace MassTransit.Transports
{
    public class ExecuteActivityReceiveEndpointDispatcher<TActivity, TArguments> :
        ITypeReceiveEndpointDispatcherFactory
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        public IReceiveEndpointDispatcher Create(IReceiveEndpointDispatcherFactory factory, IEndpointNameFormatter formatter)
        {
            var queueName = formatter.ExecuteActivity<TActivity, TArguments>();

            return factory.CreateExecuteActivityReceiver<TActivity>(queueName);
        }
    }
}
