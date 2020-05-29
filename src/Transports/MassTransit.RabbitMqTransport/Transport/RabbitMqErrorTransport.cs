namespace MassTransit.RabbitMqTransport.Transport
{
    using System.Threading.Tasks;
    using GreenPipes;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqErrorTransport :
        RabbitMqMoveTransport,
        IErrorTransport
    {
        public RabbitMqErrorTransport(string exchange, IFilter<ModelContext> topologyFilter)
            : base(exchange, topologyFilter)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(IBasicProperties message, SendHeaders headers)
            {
                headers.SetExceptionHeaders(context);

                message.ClearExpiration();
            }

            return Move(context, PreSend);
        }
    }
}
