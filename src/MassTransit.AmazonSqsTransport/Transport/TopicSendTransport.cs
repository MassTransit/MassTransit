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


        struct SendPipe<T> :
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

            public async Task Send(ClientContext clientContext)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                await _context.ConfigureTopologyPipe.Send(clientContext).ConfigureAwait(false);

                var context = new TransportAmazonSqsSendContext<T>(_message, _cancellationToken);

                var activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartActivity(new {_context.EntityName});
                try
                {
                    await _pipe.Send(context).ConfigureAwait(false);

                    activity.AddSendContextHeaders(context);

                    var transportMessage = clientContext.CreatePublishRequest(_context.EntityName, context.Body);

                    transportMessage.MessageAttributes.Set(context.Headers);

                    transportMessage.MessageAttributes.Set("Content-Type", context.ContentType.MediaType);
                    transportMessage.MessageAttributes.Set(nameof(context.MessageId), context.MessageId);
                    transportMessage.MessageAttributes.Set(nameof(context.CorrelationId), context.CorrelationId);
                    transportMessage.MessageAttributes.Set(nameof(context.TimeToLive), context.TimeToLive);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                    await clientContext.Publish(transportMessage, context.CancellationToken).ConfigureAwait(false);

                    context.LogSent();

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PostSend(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    context.LogFaulted(ex);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.SendFault(context, ex).ConfigureAwait(false);

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
