namespace MassTransit.Conductor.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Server;


    public class LinkConsumer<T> :
        IConsumer<Link<T>>
        where T : class
    {
        readonly ILogger _logger = Logger.Get<LinkConsumer<T>>();
        readonly IMessageEndpoint<T> _messageEndpoint;

        public LinkConsumer(IMessageEndpoint<T> messageEndpoint)
        {
            _messageEndpoint = messageEndpoint;
        }

        public Task Consume(ConsumeContext<Link<T>> context)
        {
            _logger.LogDebug("Linking ClientId: {0}", context.Message.ClientId);

            return context.RespondAsync<Up<T>>(new
            {
                _messageEndpoint.ServiceAddress,
                Endpoint = _messageEndpoint.EndpointInfo,
            });
        }
    }
}
