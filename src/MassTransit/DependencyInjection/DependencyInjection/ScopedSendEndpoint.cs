namespace MassTransit.DependencyInjection
{
    using System;
    using Transports;


    public class ScopedSendEndpoint :
        SendEndpointProxy
    {
        readonly IServiceProvider _scope;

        public ScopedSendEndpoint(ISendEndpoint endpoint, IServiceProvider scope)
            : base(endpoint)
        {
            _scope = scope;
        }

        protected override IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe = default)
        {
            return new ScopedSendPipeAdapter<T>(_scope, pipe);
        }
    }
}
