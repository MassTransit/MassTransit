namespace MassTransit.Services.LegacyProxy.Threading
{
    using System;
    using System.Threading;
    using Events;
    using Internal;
    using log4net;
    using Magnum.Extensions;
    using Magnum.Pipeline;

    public class EndpointThreadPoolConsumerPool :
        ConsumerPool
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ThreadPoolConsumerPool));

        private readonly IEndpoint _bus;
        private readonly Pipe _eventAggregator;
        private int _consumerCount;
        private bool _disposed;
        private bool _enabled;
        private int _maximumThreadCount = 25;
        private int _receiverCount;
        private ISubscriptionScope _scope;
        private readonly TimeSpan _receiveTimeout;
        private readonly object _locker = new object();
        readonly Action<object> _workToDo;


        public EndpointThreadPoolConsumerPool(IEndpoint bus, Pipe eventAggregator, TimeSpan receiveTimeout, Action<object> workToDo)
		{
			_receiveTimeout = receiveTimeout;
            _workToDo = workToDo;
            _eventAggregator = eventAggregator;
			_bus = bus;
		}



        public int MaximumConsumerCount
        {
            get { return _maximumThreadCount; }
            set
            {
                if (value <= 0)
                    throw new InvalidOperationException("The maximum thread count must be at least one");

                _maximumThreadCount = value;
            }
        }

        public void Start()
        {
            _scope = _eventAggregator.NewSubscriptionScope();
            _scope.Subscribe<ReceiveCompleted>(Handle);
            _scope.Subscribe<ConsumeCompleted>(Handle);

            _enabled = true;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting Consumer Pool for {0}", _bus.Uri);

            QueueReceiver();
        }

        public void Stop()
        {
            _enabled = false;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Stopping Consumer Pool for {0}", _bus.Uri);

            if (_consumerCount == 0)
                return;

            var completed = new AutoResetEvent(true);

            _scope.Subscribe<ConsumeCompleted>(x => completed.Set());

            while (completed.WaitOne(60.Seconds(), true))
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Consumer Pool stopped for {0}", _bus.Uri);

                if (_consumerCount == 0)
                    return;
            }

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Timeout stopping Consumer Pool for {0}", _bus.Uri);
        }

        private void Handle(ReceiveCompleted message)
        {
            Interlocked.Decrement(ref _receiverCount);

            QueueReceiver();
        }

        private void Handle(ConsumeCompleted message)
        {
            Interlocked.Decrement(ref _consumerCount);

            QueueReceiver();
        }

        private void QueueReceiver()
        {
            if (_enabled == false)
                return;

            lock (_locker)
            {
                if (_receiverCount > 0)
                    return;

                if (_consumerCount >= _maximumThreadCount)
                    return;

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Queueing receiver for {0}", _bus.Uri);

                var context = new EndpointReceiveContext(_bus, _eventAggregator, _receiveTimeout, _workToDo);

                Interlocked.Increment(ref _receiverCount);
                Interlocked.Increment(ref _consumerCount);

                try
                {
                    ThreadPool.QueueUserWorkItem(x => context.ReceiveFromEndpoint());
                }
                catch (Exception ex)
                {
                    _log.Error("Unable to queue consumer to thread pool", ex);

                    Interlocked.Decrement(ref _receiverCount);
                    Interlocked.Decrement(ref _consumerCount);
                }
            }

            _eventAggregator.Send(new ReceiverQueued
            {
                ReceiverCount = _receiverCount,
                ConsumerCount = _consumerCount,
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (_scope != null)
                {
                    _scope.Dispose();
                    _scope = null;
                }
            }

            _disposed = true;
        }

        ~EndpointThreadPoolConsumerPool()
		{
			Dispose(false);
		}
    }
}