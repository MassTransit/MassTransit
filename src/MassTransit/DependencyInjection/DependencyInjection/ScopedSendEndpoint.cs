namespace MassTransit.DependencyInjection
{
    using Transports;


    public class ScopedSendEndpoint<TScope> :
        SendEndpointProxy
        where TScope : class
    {
        readonly TScope _scope;

        public ScopedSendEndpoint(ISendEndpoint endpoint, TScope scope)
            : base(endpoint)
        {
            _scope = scope;
        }

        protected override IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe = default)
        {
            return new ScopedSendPipeAdapter<TScope, T>(_scope, pipe);
        }
    }
}
