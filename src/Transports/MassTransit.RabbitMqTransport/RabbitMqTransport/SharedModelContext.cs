namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Middleware;
    using RabbitMQ.Client;


    public class SharedModelContext :
        ProxyPipeContext,
        ModelContext
    {
        readonly ModelContext _context;

        public SharedModelContext(ModelContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        ConnectionContext ModelContext.ConnectionContext => _context.ConnectionContext;

        Task ModelContext.BasicPublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body, bool awaitAck)
        {
            return _context.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, body, awaitAck);
        }

        Task ModelContext.ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            return _context.ExchangeBind(destination, source, routingKey, arguments);
        }

        Task ModelContext.ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _context.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }

        public Task ExchangeDeclarePassive(string exchange)
        {
            return _context.ExchangeDeclarePassive(exchange);
        }

        Task ModelContext.QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            return _context.QueueBind(queue, exchange, routingKey, arguments);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return _context.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclarePassive(string queue)
        {
            return _context.QueueDeclarePassive(queue);
        }

        Task<uint> ModelContext.QueuePurge(string queue)
        {
            return _context.QueuePurge(queue);
        }

        Task ModelContext.BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            return _context.BasicQos(prefetchSize, prefetchCount, global);
        }

        Task ModelContext.BasicAck(ulong deliveryTag, bool multiple)
        {
            return _context.BasicAck(deliveryTag, multiple);
        }

        Task ModelContext.BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            return _context.BasicNack(deliveryTag, multiple, requeue);
        }

        Task<string> ModelContext.BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer,
            string consumerTag)
        {
            return _context.BasicConsume(queue, noAck, exclusive, arguments, consumer, consumerTag);
        }

        public Task BasicCancel(string consumerTag)
        {
            return _context.BasicCancel(consumerTag);
        }

        IModel ModelContext.Model => _context.Model;
    }
}
