namespace MassTransit.Middleware
{
    using System;


    public class RequestIdTeeFilter<TMessage> :
        TeeFilter<ConsumeContext<TMessage>>,
        IRequestIdTeeFilter<TMessage>
        where TMessage : class
    {
        readonly Lazy<IKeyPipeConnector<TMessage, Guid>> _keyConnections;

        public RequestIdTeeFilter()
        {
            _keyConnections = new Lazy<IKeyPipeConnector<TMessage, Guid>>(ConnectKeyFilter);
        }

        public ConnectHandle ConnectPipe(Guid key, IPipe<ConsumeContext<TMessage>> pipe)
        {
            return _keyConnections.Value.ConnectPipe(key, pipe);
        }

        RequestIdFilter<TMessage> ConnectKeyFilter()
        {
            var filter = new RequestIdFilter<TMessage>();

            ConnectPipe(filter.ToPipe());

            return filter;
        }
    }
}
