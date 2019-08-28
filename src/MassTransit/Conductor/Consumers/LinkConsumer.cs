namespace MassTransit.Conductor.Consumers
{
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Server;


    public class LinkConsumer<T> :
        IConsumer<Link<T>>
        where T : class
    {
        readonly IMessageEndpoint<T> _messageEndpoint;

        public LinkConsumer(IMessageEndpoint<T> messageEndpoint)
        {
            _messageEndpoint = messageEndpoint;
        }

        public async Task Consume(ConsumeContext<Link<T>> context)
        {
            LogContext.Debug?.Log("Linking Client: {Id}", context.Message.ClientId);

            await _messageEndpoint.Link(context.Message.ClientId, context.SourceAddress).ConfigureAwait(false);

            await context.RespondAsync<Up<T>>(new
            {
                _messageEndpoint.ServiceAddress,
                Endpoint = _messageEndpoint.EndpointInfo,
            }).ConfigureAwait(false);
        }
    }
}
