namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using Transports;


    public class TopicSendTransport :
        Supervisor,
        ISendTransport
    {
        readonly SqsSendTransportContext _context;

        public TopicSendTransport(SqsSendTransportContext context)
        {
            _context = context;

            Add(context.ClientContextSupervisor);
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The send transport is stopped: {_context.EntityName}");

            var sendPipe = new SendPipe<T>(_context, message, pipe, cancellationToken);

            return _context.ClientContextSupervisor.Send(sendPipe, cancellationToken);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }


        class SendPipe<T> :
            IPipe<ClientContext>
            where T : class
        {
            readonly SqsSendTransportContext _context;
            readonly T _message;
            readonly IPipe<SendContext<T>> _pipe;
            readonly CancellationToken _cancellationToken;

            public SendPipe(SqsSendTransportContext context, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            {
                _context = context;
                _message = message;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ClientContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                await _context.ConfigureTopologyPipe.Send(context).ConfigureAwait(false);

                var sendContext = new TransportAmazonSqsSendContext<T>(_message, _cancellationToken);

                await _pipe.Send(sendContext).ConfigureAwait(false);

                var activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(sendContext);
                try
                {
                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(sendContext).ConfigureAwait(false);

                    var request = await context.CreatePublishRequest(_context.EntityName, sendContext.Body).ConfigureAwait(false);

                    _context.SnsSetHeaderAdapter.Set(request.MessageAttributes, sendContext.Headers);

                    _context.SnsSetHeaderAdapter.Set(request.MessageAttributes, "Content-Type", sendContext.ContentType.MediaType);
                    _context.SnsSetHeaderAdapter.Set(request.MessageAttributes, nameof(sendContext.CorrelationId), sendContext.CorrelationId);

                    await context.Publish(request, sendContext.CancellationToken).ConfigureAwait(false);

                    sendContext.LogSent();

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PostSend(sendContext).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    sendContext.LogFaulted(ex);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.SendFault(sendContext, ex).ConfigureAwait(false);

                    throw;
                }
                finally
                {
                    activity?.Stop();
                }
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
