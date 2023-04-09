namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Middleware;


    public class SendTransport<TContext> :
        Supervisor,
        ISendTransport,
        IAsyncDisposable
        where TContext : class, PipeContext
    {
        readonly SendTransportContext<TContext> _context;

        public SendTransport(SendTransportContext<TContext> context)
        {
            _context = context;

            foreach (var agent in context.GetAgentHandles())
                Add(agent);
        }

        public async ValueTask DisposeAsync()
        {
            await this.Stop("Disposed").ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            return _context.CreateSendContext(message, pipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The send transport is stopped: {_context.EntityName}");

            LogContext.SetCurrentIfNull(_context.LogContext);

            var sendPipe = new SendPipe<T>(_context, message, pipe, cancellationToken);

            return _context.Send(sendPipe, cancellationToken);
        }

        protected override Task StopSupervisor(StopSupervisorContext context)
        {
            TransportLogMessages.StoppingSendTransport(_context.EntityName);

            return base.StopSupervisor(context);
        }


        class SendPipe<T> :
            IPipe<TContext>
            where T : class
        {
            readonly CancellationToken _cancellationToken;
            readonly T _message;
            readonly IPipe<SendContext<T>> _pipe;
            readonly SendTransportContext<TContext> _sendTransportContext;

            public SendPipe(SendTransportContext<TContext> sendTransportContext, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            {
                _sendTransportContext = sendTransportContext;
                _message = message;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(TContext context)
            {
                SendContext<T> sendContext = await _sendTransportContext.CreateSendContext(context, _message, _pipe, _cancellationToken).ConfigureAwait(false);

                StartedActivity? activity = LogContext.Current?.StartSendActivity(_sendTransportContext, sendContext);
                StartedInstrument? instrument = LogContext.Current?.StartSendInstrument(_sendTransportContext, sendContext);
                try
                {
                    if (_sendTransportContext.SendObservers.Count > 0)
                        await _sendTransportContext.SendObservers.PreSend(sendContext).ConfigureAwait(false);

                    await _sendTransportContext.Send(context, sendContext).ConfigureAwait(false);

                    activity?.Update(sendContext);
                    sendContext.LogSent();

                    if (_sendTransportContext.SendObservers.Count > 0)
                        await _sendTransportContext.SendObservers.PostSend(sendContext).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    sendContext.LogFaulted(ex);

                    if (_sendTransportContext.SendObservers.Count > 0)
                        await _sendTransportContext.SendObservers.SendFault(sendContext, ex).ConfigureAwait(false);

                    activity?.AddExceptionEvent(ex);
                    instrument?.AddException(ex);

                    throw;
                }
                finally
                {
                    activity?.Stop();
                    instrument?.Stop();
                }
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
