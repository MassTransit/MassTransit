namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Logging;
    using Observables;


    public class ReceivePipeDispatcher :
        IReceivePipeDispatcher
    {
        readonly string _activityName;
        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveObservable _observers;
        readonly IReceivePipe _receivePipe;

        int _activeDispatchCount;
        long _dispatchCount;
        int _maxConcurrentDispatchCount;
        readonly string _endpointName;
        readonly string _inputAddress;

        public ReceivePipeDispatcher(IReceivePipe receivePipe, ReceiveObservable observers, IHostConfiguration hostConfiguration, Uri inputAddress)
        {
            _receivePipe = receivePipe;
            _observers = observers;
            _hostConfiguration = hostConfiguration;

            _inputAddress = inputAddress.ToString();
            _activityName = $"{inputAddress.GetDiagnosticEndpointName()} receive";
            _endpointName = inputAddress.GetEndpointName();
        }

        public int ActiveDispatchCount => _activeDispatchCount;
        public long DispatchCount => _dispatchCount;
        public int MaxConcurrentDispatchCount => _maxConcurrentDispatchCount;

        public DeliveryMetrics GetMetrics()
        {
            return new Metrics(_dispatchCount, _maxConcurrentDispatchCount);
        }

        public event ZeroActiveDispatchHandler ZeroActivity;

        public async Task Dispatch(ReceiveContext context, ReceiveLockContext receiveLock = default)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            var active = StartDispatch();

            StartedActivity? activity = LogContext.Current?.StartReceiveActivity(_activityName, _inputAddress, _endpointName, context);
            try
            {
                if (_observers.Count > 0)
                    await _observers.PreReceive(context).ConfigureAwait(false);

                if (receiveLock != null)
                    await receiveLock.ValidateLockStatus().ConfigureAwait(false);

                await _receivePipe.Send(context).ConfigureAwait(false);

                await context.ReceiveCompleted.ConfigureAwait(false);

                if (receiveLock != null)
                    await receiveLock.Complete().ConfigureAwait(false);

                if (_observers.Count > 0)
                    await _observers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_observers.Count > 0)
                    await _observers.ReceiveFault(context, ex).ConfigureAwait(false);

                if (receiveLock != null)
                {
                    try
                    {
                        await receiveLock.Faulted(ex).ConfigureAwait(false);
                    }
                    catch (Exception releaseLockException)
                    {
                        throw new AggregateException("ReceiveLock.Faulted threw an exception", releaseLockException, ex);
                    }
                }

                throw;
            }
            finally
            {
                activity?.Stop();

                await active.Complete().ConfigureAwait(false);
            }
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _observers.Connect(observer);
        }

        public void Probe(ProbeContext context)
        {
            _receivePipe.Probe(context);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _receivePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _receivePipe.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _receivePipe.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receivePipe.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _receivePipe.ConnectConsumeMessageObserver(observer);
        }

        ActiveDispatch StartDispatch()
        {
            var current = Interlocked.Increment(ref _activeDispatchCount);
            while (current > _maxConcurrentDispatchCount)
                Interlocked.CompareExchange(ref _maxConcurrentDispatchCount, current, _maxConcurrentDispatchCount);

            return new ActiveDispatch(Interlocked.Increment(ref _dispatchCount), DispatchComplete);
        }

        async Task DispatchComplete(long id)
        {
            var pendingCount = Interlocked.Decrement(ref _activeDispatchCount);
            if (pendingCount == 0)
            {
                var zeroActivity = ZeroActivity;
                if (zeroActivity != null)
                {
                    foreach (var @delegate in zeroActivity.GetInvocationList())
                    {
                        if (@delegate is ZeroActiveDispatchHandler handler)
                            await handler().ConfigureAwait(false);
                    }
                }
            }
        }


        readonly struct ActiveDispatch
        {
            readonly long _id;
            readonly Func<long, Task> _complete;

            public ActiveDispatch(long id, Func<long, Task> complete)
            {
                _id = id;
                _complete = complete;
            }

            public Task Complete()
            {
                return _complete(_id);
            }
        }


        class Metrics :
            DeliveryMetrics
        {
            public Metrics(long deliveryCount, int concurrentDeliveryCount)
            {
                DeliveryCount = deliveryCount;
                ConcurrentDeliveryCount = concurrentDeliveryCount;
            }

            public long DeliveryCount { get; }
            public int ConcurrentDeliveryCount { get; }
        }
    }
}
