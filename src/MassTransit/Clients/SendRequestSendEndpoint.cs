namespace MassTransit.Clients
{
    using System;
    using System.Threading.Tasks;


    public class SendRequestSendEndpoint<TRequest> :
        RequestSendEndpoint<TRequest>
        where TRequest : class
    {
        readonly Uri _destinationAddress;
        readonly ISendEndpointProvider _provider;

        public SendRequestSendEndpoint(ISendEndpointProvider provider, Uri destinationAddress, ConsumeContext consumeContext)
            : base(consumeContext)
        {
            _provider = provider;
            _destinationAddress = destinationAddress;
        }

        protected override async Task<ISendEndpoint> GetSendEndpoint()
        {
            var endpoint = await _provider.GetSendEndpoint(_destinationAddress).ConfigureAwait(false);

            return endpoint.SkipOutbox();
        }
    }
}
