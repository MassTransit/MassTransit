using Amazon.SQS.Model;
using MassTransit.Transports;
using System;
using System.Threading.Tasks;

namespace MassTransit.AmazonSqsTransport
{
    public class MessagesHandler
    {
        public delegate void Action();

        private readonly ClientContext _client;
        private readonly SqsReceiveEndpointContext _context;
        private readonly ReceiveSettings _receiveSettings;
        private readonly IReceivePipeDispatcher _dispatcher;

        public MessagesHandler(ClientContext client, SqsReceiveEndpointContext context, IReceivePipeDispatcher dispatcher)
        {
            _client = client;
            _context = context;
            _dispatcher = dispatcher;

            _receiveSettings = client.GetPayload<ReceiveSettings>();
        }

        public async Task Run(Message message, Action onCompletedCallback)
        {
            var redelivered = message.Attributes.TryGetInt("ApproximateReceiveCount", out var receiveCount) && receiveCount > 1;

            var context = new AmazonSqsReceiveContext(message, redelivered, _context, _client, _receiveSettings, _client.ConnectionContext);
            try
            {
                await _dispatcher.Dispatch(context, context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogTransportFaulted(exception);
            }
            finally
            {
                context.Dispose();
                onCompletedCallback();
            }
        }
    }
}
