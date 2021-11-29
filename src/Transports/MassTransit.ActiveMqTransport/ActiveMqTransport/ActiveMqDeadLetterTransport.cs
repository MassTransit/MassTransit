namespace MassTransit.ActiveMqTransport
{
    using System.Threading.Tasks;
    using Apache.NMS;
    using Transports;


    public class ActiveMqDeadLetterTransport :
        ActiveMqMoveTransport,
        IDeadLetterTransport
    {
        public ActiveMqDeadLetterTransport(string destination, IFilter<SessionContext> topologyFilter)
            : base(destination, topologyFilter)
        {
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(IMessage message, SendHeaders headers)
            {
                headers.Set(MessageHeaders.Reason, reason ?? "Unspecified");
            }

            return Move(context, PreSend);
        }
    }
}
