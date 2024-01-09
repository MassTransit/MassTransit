#nullable enable
namespace MassTransit.SqlTransport
{
    using System.Threading.Tasks;
    using Transports;


    public class SqlQueueDeadLetterTransport :
        SqlQueueMoveTransport,
        IDeadLetterTransport
    {
        public SqlQueueDeadLetterTransport(string queueName, SqlQueueType queueType)
            : base(queueName, queueType)
        {
        }

        public Task Send(ReceiveContext context, string? reason)
        {
            void PreSend(SqlTransportMessage message, SendHeaders headers)
            {
                headers.Set(MessageHeaders.Reason, reason ?? "Unspecified");
            }

            return Move(context, PreSend);
        }
    }
}
