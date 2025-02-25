namespace MassTransit.RabbitMqTransport
{
    using System.Threading.Tasks;
    using Middleware;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqDeadLetterTransport :
        RabbitMqMoveTransport<DeadLetterSettings>,
        IDeadLetterTransport
    {
        public RabbitMqDeadLetterTransport(string exchange, ConfigureRabbitMqTopologyFilter<DeadLetterSettings> topologyFilter)
            : base(exchange, topologyFilter)
        {
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(BasicProperties message, SendHeaders headers)
            {
                headers.Set(MessageHeaders.Reason, reason ?? "Unspecified");
            }

            return Move(context, PreSend);
        }
    }
}
