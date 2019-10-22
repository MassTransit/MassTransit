namespace MassTransit.Clients.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class BusClientFactoryContext :
        ClientFactoryContext
    {
        readonly IBus _bus;

        public BusClientFactoryContext(IBus bus, RequestTimeout defaultTimeout = default)
        {
            _bus = bus;

            DefaultTimeout = defaultTimeout.HasValue ? defaultTimeout : RequestTimeout.Default;
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _bus.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _bus.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _bus.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _bus.GetSendEndpoint(address);
        }

        public Uri ResponseAddress => _bus.Address;

        public IPublishEndpoint PublishEndpoint => _bus;

        public RequestTimeout DefaultTimeout { get; }
    }
}
