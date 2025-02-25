namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using RabbitMQ.Client;


    public class ScopeChannelContext :
        ScopePipeContext,
        ChannelContext
    {
        readonly ChannelContext _context;

        public ScopeChannelContext(ChannelContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public IChannel Channel => _context.Channel;

        ConnectionContext ChannelContext.ConnectionContext => _context.ConnectionContext;

        Task ChannelContext.BasicPublishAsync(string exchange, string routingKey, bool mandatory, BasicProperties basicProperties, byte[] body, bool awaitAck)
        {
            return _context.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, body, awaitAck);
        }

        Task ChannelContext.ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            return _context.ExchangeBind(destination, source, routingKey, arguments);
        }

        Task ChannelContext.ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _context.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }

        public Task ExchangeDeclarePassive(string exchange)
        {
            return _context.ExchangeDeclarePassive(exchange);
        }

        Task ChannelContext.QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            return _context.QueueBind(queue, exchange, routingKey, arguments);
        }

        Task<QueueDeclareOk> ChannelContext.QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _context.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }

        Task<QueueDeclareOk> ChannelContext.QueueDeclarePassive(string queue)
        {
            return _context.QueueDeclarePassive(queue);
        }

        Task<uint> ChannelContext.QueuePurge(string queue)
        {
            return _context.QueuePurge(queue);
        }

        Task ChannelContext.BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            return _context.BasicQos(prefetchSize, prefetchCount, global);
        }

        ValueTask ChannelContext.BasicAck(ulong deliveryTag, bool multiple)
        {
            return _context.BasicAck(deliveryTag, multiple);
        }

        Task ChannelContext.BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            return _context.BasicNack(deliveryTag, multiple, requeue);
        }

        public Task<string> BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IAsyncBasicConsumer consumer,
            string consumerTag, CancellationToken cancellationToken)
        {
            return _context.BasicConsume(queue, noAck, exclusive, arguments, consumer, consumerTag, cancellationToken);
        }

        public Task BasicCancel(string consumerTag)
        {
            return _context.BasicCancel(consumerTag);
        }
    }
}
