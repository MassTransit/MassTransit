namespace MassTransit.Clients
{
    using System.Threading.Tasks;


    public class PublishRequestSendEndpoint<TRequest> :
        RequestSendEndpoint<TRequest>
        where TRequest : class
    {
        readonly IPublishEndpointProvider _provider;

        public PublishRequestSendEndpoint(IPublishEndpointProvider provider, ConsumeContext consumeContext)
            : base(consumeContext)
        {
            _provider = provider;
        }

        protected override async Task<ISendEndpoint> GetSendEndpoint()
        {
            var endpoint = await _provider.GetPublishSendEndpoint<TRequest>().ConfigureAwait(false);

            return endpoint.SkipOutbox();
        }
    }
}
