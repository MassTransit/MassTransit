namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;
    using MassTransit.ActiveMqTransport.Topology;
    using System.Threading.Tasks;
    using Transports;


    public class ActiveMqErrorTransport :
        ActiveMqMoveTransport,
        IErrorTransport
    {
        public ActiveMqErrorTransport(Queue destination, IFilter<SessionContext> topologyFilter)
            : base(destination, topologyFilter)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(IMessage message, SendHeaders headers)
            {
                headers.SetExceptionHeaders(context);
            }

            return Move(context, PreSend);
        }
    }
}
