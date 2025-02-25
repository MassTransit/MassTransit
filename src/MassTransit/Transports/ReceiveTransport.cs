namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Middleware;


    public class ReceiveTransport<TContext> :
        IReceiveTransport
        where TContext : class, PipeContext
    {
        readonly ReceiveEndpointContext _context;
        readonly IHostConfiguration _hostConfiguration;
        readonly Func<ITransportSupervisor<TContext>> _supervisorFactory;
        readonly IPipe<TContext> _transportPipe;

        public ReceiveTransport(IHostConfiguration hostConfiguration, ReceiveEndpointContext context, Func<ITransportSupervisor<TContext>> supervisorFactory,
            IPipe<TContext> transportPipe)
        {
            _hostConfiguration = hostConfiguration;
            _context = context;
            _supervisorFactory = supervisorFactory;
            _transportPipe = transportPipe;
        }

        public IPipe<TContext> PreStartPipe { get; set; }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiveTransport");

            _context.Probe(scope);
        }

        /// <summary>
        /// Start the receive transport, returning a Task that can be awaited to signal the transport has
        /// completely shutdown once the cancellation token is cancelled.
        /// </summary>
        /// <returns>A task that is completed once the transport is shut down</returns>
        public ReceiveTransportHandle Start()
        {
            return new ReceiveTransportAgent(_hostConfiguration.ReceiveTransportRetryPolicy, _context, _supervisorFactory, _transportPipe, PreStartPipe);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _context.ConnectReceiveTransportObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }


        class ReceiveTransportAgent :
            Agent,
            ReceiveTransportHandle
        {
            readonly ReceiveEndpointContext _context;
            readonly IPipe<TContext> _preStartPipe;
            readonly IRetryPolicy _retryPolicy;
            readonly Func<ITransportSupervisor<TContext>> _supervisorFactory;
            readonly IPipe<TContext> _transportPipe;
            ITransportSupervisor<TContext> _supervisor;

            public ReceiveTransportAgent(IRetryPolicy retryPolicy, ReceiveEndpointContext context, Func<ITransportSupervisor<TContext>> supervisorFactory,
                IPipe<TContext> transportPipe, IPipe<TContext> preStartPipe)
            {
                _retryPolicy = retryPolicy;
                _context = context;
                _supervisorFactory = supervisorFactory;
                _transportPipe = transportPipe;
                _preStartPipe = preStartPipe;

                var receiver = Task.Run(Run);

                SetReady(receiver);

                SetCompleted(receiver);
            }

            public Task Stop(CancellationToken cancellationToken)
            {
                return this.Stop("Stop Receive Transport", cancellationToken);
            }

            protected override async Task StopAgent(StopContext context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                if (_supervisor != null)
                    await _supervisor.Stop(context).ConfigureAwait(false);

                await Completed.ConfigureAwait(false);
            }

            async Task Run()
            {
                async Task DelayWithCancellation(TimeSpan delay)
                {
                    try
                    {
                        await Task.Delay(delay, Stopping).ConfigureAwait(false);
                    }
                    catch
                    {
                        // just a little breather before reconnecting the receive transport
                    }
                }

                var stoppingContext = new TransportStoppingContext(Stopping);

                RetryPolicyContext<TransportStoppingContext> policyContext = _retryPolicy.CreatePolicyContext(stoppingContext);

                try
                {
                    RetryContext<TransportStoppingContext> retryContext = null;

                    while (!IsStopping)
                    {
                        try
                        {
                            if (retryContext != null)
                            {
                                LogContext.Info?.Log(retryContext.Exception, "Retrying {Delay}: {Message}", retryContext.Delay, retryContext.Exception.Message);

                                if (retryContext.Delay != null)
                                    await DelayWithCancellation(retryContext.Delay.Value).ConfigureAwait(false);
                            }

                            if (!IsStopping)
                                await RunTransport().ConfigureAwait(false);
                        }
                        catch (OperationCanceledException exception)
                        {
                            if (retryContext == null)
                                await NotifyFaulted(exception).ConfigureAwait(false);

                            throw;
                        }
                        catch (Exception exception)
                        {
                            if (retryContext != null)
                            {
                                retryContext = retryContext.CanRetry(exception, out RetryContext<TransportStoppingContext> nextRetryContext)
                                    ? nextRetryContext
                                    : null;
                            }

                            if (retryContext == null && !policyContext.CanRetry(exception, out retryContext))
                            {
                                LogContext.Error?.Log(exception, "ReceiveTransport Cannot Retry: {InputAddress}", _context.InputAddress);
                                break;
                            }
                        }

                        if (IsStopping)
                            break;

                        await DelayWithCancellation(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException exception)
                {
                    if (exception.CancellationToken != Stopping)
                        LogContext.Debug?.Log(exception, "ReceiveTransport Operation Cancelled: {InputAddress}", _context.InputAddress);
                }
                catch (Exception exception)
                {
                    LogContext.Debug?.Log(exception, "ReceiveTransport Run Exception: {InputAddress}", _context.InputAddress);
                }
                finally
                {
                    policyContext.Dispose();
                }
            }

            async Task RunTransport()
            {
                try
                {
                    _supervisor = _supervisorFactory();

                    if (_preStartPipe.IsNotEmpty())
                        await _supervisor.Send(_preStartPipe, Stopping).ConfigureAwait(false);

                    // Nothing connected to the pipe, so signal early we are available
                    if (!_context.ReceivePipe.Connected.IsCompleted)
                        await _context.OnTransportStartup(_supervisor, Stopping).ConfigureAwait(false);

                    if (!IsStopping)
                        await _supervisor.Send(_transportPipe, Stopped).ConfigureAwait(false);
                }
                catch (ConnectionException exception)
                {
                    await NotifyFaulted(exception).ConfigureAwait(false);
                    throw;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw await NotifyFaulted(exception, "ReceiveTransport faulted: ").ConfigureAwait(false);
                }
            }

            async Task<Exception> NotifyFaulted(Exception originalException, string message)
            {
                var exception = _context.ConvertException(originalException, message);

                await NotifyFaulted(exception).ConfigureAwait(false);

                return exception;
            }

            Task NotifyFaulted(Exception exception)
            {
                return _context.TransportObservers.NotifyFaulted(_context.InputAddress, exception);
            }


            class TransportStoppingContext :
                BasePipeContext
            {
                public TransportStoppingContext(CancellationToken cancellationToken)
                    : base(cancellationToken)
                {
                }
            }
        }
    }
}
