namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using Transports;


    public class ActiveMqSendTransport :
        Supervisor,
        ISendTransport,
        IAsyncDisposable
    {
        readonly ActiveMqSendTransportContext _context;

        public ActiveMqSendTransport(ActiveMqSendTransportContext context)
        {
            _context = context;

            Add(context.SessionContextSupervisor);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return this.Stop("Disposed", cancellationToken);
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The send transport is stopped: {_context.EntityName}/{_context.DestinationType}");

            var sendPipe = new SendPipe<T>(_context, message, pipe, cancellationToken);

            return _context.SessionContextSupervisor.Send(sendPipe, cancellationToken);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        protected override Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping send transport: {EntityName}", _context.EntityName);

            return base.StopSupervisor(context);
        }


        struct SendPipe<T> :
            IPipe<SessionContext>
            where T : class
        {
            readonly ActiveMqSendTransportContext _context;
            readonly T _message;
            readonly IPipe<SendContext<T>> _pipe;
            readonly CancellationToken _cancellationToken;

            public SendPipe(ActiveMqSendTransportContext context, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            {
                _context = context;
                _message = message;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(SessionContext sessionContext)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                await _context.ConfigureTopologyPipe.Send(sessionContext).ConfigureAwait(false);

                var destination = await sessionContext.GetDestination(_context.EntityName, _context.DestinationType).ConfigureAwait(false);
                var producer = await sessionContext.CreateMessageProducer(destination).ConfigureAwait(false);

                var context = new TransportActiveMqSendContext<T>(_message, _cancellationToken);

                var activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartActivity(new
                {
                    _context.EntityName,
                    _context.DestinationType
                });
                try
                {
                    await _pipe.Send(context).ConfigureAwait(false);

                    activity.AddSendContextHeaders(context);

                    byte[] body = context.Body;

                    var transportMessage = sessionContext.Session.CreateBytesMessage();

                    transportMessage.Properties.SetHeaders(context.Headers);

                    transportMessage.Properties["Content-Type"] = context.ContentType.MediaType;

                    transportMessage.NMSDeliveryMode = context.Durable ? MsgDeliveryMode.Persistent : MsgDeliveryMode.NonPersistent;

                    if (context.MessageId.HasValue)
                        transportMessage.NMSMessageId = context.MessageId.ToString();

                    if (context.CorrelationId.HasValue)
                        transportMessage.NMSCorrelationID = context.CorrelationId.ToString();

                    if (context.TimeToLive.HasValue)
                        transportMessage.NMSTimeToLive = context.TimeToLive.Value;

                    if (context.Priority.HasValue)
                        transportMessage.NMSPriority = context.Priority.Value;

                    transportMessage.Content = body;

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                    var publishTask = Task.Run(() => producer.Send(transportMessage), context.CancellationToken);

                    await publishTask.OrCanceled(context.CancellationToken).ConfigureAwait(false);

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
