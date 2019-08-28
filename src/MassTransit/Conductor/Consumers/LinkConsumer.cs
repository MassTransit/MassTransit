namespace MassTransit.Conductor.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using Server;


    public class LinkConsumer<T> :
        IConsumer<Link<T>>
        where T : class
    {
        readonly ILog _log = Logger.Get<LinkConsumer<T>>();
        readonly IMessageEndpoint<T> _messageEndpoint;

        public LinkConsumer(IMessageEndpoint<T> messageEndpoint)
        {
            _messageEndpoint = messageEndpoint;
        }

        public async Task Consume(ConsumeContext<Link<T>> context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Linking ClientId: {0}", context.Message.ClientId);

            await _messageEndpoint.Link(context.Message.ClientId, context.SourceAddress).ConfigureAwait(false);

            await context.RespondAsync<Up<T>>(new
            {
                _messageEndpoint.ServiceAddress,
                Endpoint = _messageEndpoint.EndpointInfo,
            }).ConfigureAwait(false);
        }
    }
}
