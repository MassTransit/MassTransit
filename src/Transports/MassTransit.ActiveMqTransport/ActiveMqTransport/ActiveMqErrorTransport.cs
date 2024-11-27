namespace MassTransit.ActiveMqTransport
{
    using System.Threading.Tasks;
    using Apache.NMS;
    using Middleware;
    using Topology;
    using Transports;


    public class ActiveMqErrorTransport :
        ActiveMqMoveTransport<ErrorSettings>,
        IErrorTransport
    {
        public ActiveMqErrorTransport(Queue destination, ConfigureActiveMqTopologyFilter<ErrorSettings> topologyFilter)
            : base(destination, topologyFilter)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(IMessage message, SendHeaders headers)
            {
                headers.CopyFrom(context.ExceptionHeaders);
            }

            return Move(context, PreSend);
        }
    }
}
