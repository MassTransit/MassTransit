namespace MassTransit.ServiceBus.Internal
{
    using System;
    using log4net;

    public class PoisonEndpointDecorator :
        IEndpoint
    {
        private ILog _log = LogManager.GetLogger(typeof (PoisonEndpointDecorator));
        private readonly IEndpoint _wrappedEndpoint;

        public PoisonEndpointDecorator(IEndpoint wrappedEndpoint)
        {
            _wrappedEndpoint = wrappedEndpoint;
        }

        public void Dispose()
        {
            _wrappedEndpoint.Dispose();
        }

        public Uri Uri
        {
            get { return _wrappedEndpoint.Uri; }
        }

        public void Send<T>(T message) where T : class
        {
            _log.Warn("Poison Message");
            _wrappedEndpoint.Send(message);
        }

        public void Send<T>(T message, TimeSpan timeToLive) where T : class
        {
            _log.Warn("Poison Message");
            _wrappedEndpoint.Send(message, timeToLive);
        }

        public object Receive(TimeSpan timeout)
        {
            return _wrappedEndpoint.Receive(timeout);
        }

        public object Receive(TimeSpan timeout, Predicate<object> accept)
        {
            return _wrappedEndpoint.Receive(timeout, accept);
        }
    }
}