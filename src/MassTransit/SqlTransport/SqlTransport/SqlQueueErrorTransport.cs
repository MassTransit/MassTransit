namespace MassTransit.SqlTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class SqlQueueErrorTransport :
        SqlQueueMoveTransport,
        IErrorTransport
    {
        public SqlQueueErrorTransport(string queueName, SqlQueueType queueType)
            : base(queueName, queueType)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(SqlTransportMessage message, SendHeaders headers)
            {
                headers.SetExceptionHeaders(context);

                if (message.ExpirationTime.HasValue)
                    message.ExpirationTime = DateTime.UtcNow + Defaults.ErrorQueueTimeToLive;
            }

            return Move(context, PreSend);
        }
    }
}
