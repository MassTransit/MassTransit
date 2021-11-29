namespace MassTransit.Mediator.Contexts
{
    using System.Threading.Tasks;
    using Clients;


    public class MediatorRequestSendEndpoint<TRequest> :
        RequestSendEndpoint<TRequest>
        where TRequest : class
    {
        readonly ISendEndpoint _endpoint;

        public MediatorRequestSendEndpoint(ISendEndpoint endpoint, ConsumeContext consumeContext)
            : base(consumeContext)
        {
            _endpoint = endpoint;
        }

        protected override Task<ISendEndpoint> GetSendEndpoint()
        {
            return Task.FromResult(_endpoint);
        }
    }
}
