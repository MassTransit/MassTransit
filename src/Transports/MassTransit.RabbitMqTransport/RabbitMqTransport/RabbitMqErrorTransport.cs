namespace MassTransit.RabbitMqTransport
{
    using System.Threading.Tasks;
    using Middleware;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqErrorTransport :
        RabbitMqMoveTransport<ErrorSettings>,
        IErrorTransport
    {
        public RabbitMqErrorTransport(string exchange, ConfigureRabbitMqTopologyFilter<ErrorSettings> topologyFilter)
            : base(exchange, topologyFilter)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(BasicProperties message, SendHeaders headers)
            {
                headers.CopyFrom(context.ExceptionHeaders);

                message.ClearExpiration();
            }

            return Move(context, PreSend);
        }
    }
}
