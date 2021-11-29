namespace MassTransit.RabbitMqTransport
{
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqDeadLetterTransport :
        RabbitMqMoveTransport,
        IDeadLetterTransport
    {
        public RabbitMqDeadLetterTransport(string exchange, IFilter<ModelContext> topologyFilter)
            : base(exchange, topologyFilter)
        {
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(IBasicProperties message, SendHeaders headers)
            {
                headers.Set(MessageHeaders.Reason, reason ?? "Unspecified");
            }

            return Move(context, PreSend);
        }
    }
}
