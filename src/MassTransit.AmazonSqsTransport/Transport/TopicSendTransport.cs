namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
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

                var sendContext = new TransportAmazonSqsSendContext<T>(_message, _cancellationToken);
                try
                {
                    await _pipe.Send(sendContext).ConfigureAwait(false);

                    var transportMessage = clientContext.CreatePublishRequest(_context.EntityName, sendContext.Body);

                    transportMessage.MessageAttributes.Set(sendContext.Headers);

                    transportMessage.MessageAttributes.Set("Content-Type", sendContext.ContentType.MediaType);
                    transportMessage.MessageAttributes.Set(nameof(sendContext.MessageId), sendContext.MessageId);
                    transportMessage.MessageAttributes.Set(nameof(sendContext.CorrelationId), sendContext.CorrelationId);
                    transportMessage.MessageAttributes.Set(nameof(sendContext.TimeToLive), sendContext.TimeToLive);

                    await _context.SendObservers.PreSend(sendContext).ConfigureAwait(false);

                    await clientContext.Publish(transportMessage, sendContext.CancellationToken).ConfigureAwait(false);

                    sendContext.LogSent();

                    await _context.SendObservers.PostSend(sendContext).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    sendContext.LogFaulted(ex);

                    await _context.SendObservers.SendFault(sendContext, ex).ConfigureAwait(false);

                    throw;
                }
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
