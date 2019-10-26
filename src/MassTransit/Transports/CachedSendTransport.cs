namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Caching;


    public class CachedSendTransport :
        ISendTransport,
        INotifyValueUsed
    {
        readonly ISendTransport _sendTransport;
        public Uri Address { get; }

        public CachedSendTransport(Uri address, ISendTransport sendTransport)
        {
            Address = address;

            _sendTransport = sendTransport;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendTransport.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            Used?.Invoke();
            return _sendTransport.Send(message, pipe, cancellationToken);
        }

        public event Action Used;
    }
}
