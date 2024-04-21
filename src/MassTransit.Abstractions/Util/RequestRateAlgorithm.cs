namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public class RequestRateAlgorithm :
        IDisposable
    {
        public delegate IEnumerable<IGrouping<TKey, T>> GroupCallback<T, out TKey>(IEnumerable<T> results);


        public delegate IEnumerable<T> OrderCallback<T>(IEnumerable<T> results);


        public delegate Task<IEnumerable<T>> RequestCallback<T>(int resultLimit, CancellationToken cancellationToken);


        public delegate Task<int> RequestCallback(int resultLimit, CancellationToken cancellationToken);


        public delegate Task ResultCallback<in T>(T result, CancellationToken cancellationToken);


        readonly int _concurrentResultLimit;
        readonly CancellationTokenSource _disposeToken;
        readonly RequestRateAlgorithmOptions _options;
        readonly SemaphoreSlim? _rateLimitSemaphore;
        readonly Timer? _rateLimitTimer;
        readonly int _refreshThreshold;
        readonly TimeSpan _requestCancellationTimeout;
        readonly int _requestLimit;
        readonly object _requestLock = new object();
        readonly SemaphoreSlim _requestSemaphore;
        readonly int _resultLimit;
        readonly SemaphoreSlim _resultSemaphore;
        readonly Dictionary<long, Task> _tasks;

        int _activeRequestCount;
        int _count;
        bool _disposed;
        int _maxRequestCount;
        long _nextId;
        int _pendingResultCount;
        int _rateLimit;
        int _requestCount;

        public RequestRateAlgorithm(RequestRateAlgorithmOptions options)
        {
            if (options.PrefetchCount == 0)
                throw new ArgumentException("PrefetchCount must be > 0", nameof(options));
            if (options.RequestResultLimit == 0)
                throw new ArgumentException("RequestResultLimit must be > 0", nameof(options));

            _options = options;

            _requestCancellationTimeout = _options.RequestCancellationTimeout ?? TimeSpan.FromSeconds(1);

            _disposeToken = new CancellationTokenSource();
            _requestCount = 1;
            _requestSemaphore = new SemaphoreSlim(_requestCount);

            _requestLimit = (_options.PrefetchCount + _options.RequestResultLimit - 1) / _options.RequestResultLimit;

            _resultLimit = Math.Min(_options.PrefetchCount, _options.RequestResultLimit);
            _concurrentResultLimit = options.ConcurrentResultLimit ?? _requestLimit * _resultLimit;

            _refreshThreshold = 1;

            _resultSemaphore = new SemaphoreSlim(_concurrentResultLimit);

            _tasks = new Dictionary<long, Task>(_resultLimit);

            if (options.RequestRateLimit.HasValue && options.RequestRateInterval.HasValue)
            {
                _rateLimit = options.RequestRateLimit.Value;
                _rateLimitSemaphore = new SemaphoreSlim(_rateLimit);

                var interval = options.RequestRateInterval.Value;
                _rateLimitTimer = new Timer(Reset, null, interval, interval);
            }
        }

        /// <summary>
        /// The number of concurrent requests that should be performed based upon current response volume
        /// </summary>
        public int RequestCount => _requestCount;

        /// <summary>
        /// The number of results that should be requested for each request
        /// </summary>
        public int ResultLimit => _resultLimit;

        /// <summary>
        /// The current active request count
        /// </summary>
        public int ActiveRequestCount => _activeRequestCount;

        /// <summary>
        /// The maximum number of active requests that were made concurrently
        /// </summary>
        public int MaxActiveRequestCount => _maxRequestCount;

        int ActiveResultCount
        {
            get
            {
                lock (_tasks)
                    return _tasks.Count;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _rateLimitTimer?.Dispose();
            _rateLimitSemaphore?.Dispose();

            _requestSemaphore.Dispose();
            _resultSemaphore.Dispose();
        }

        /// <summary>
        /// Run a series of requests, up the limits, as a single pass
        /// </summary>
        /// <param name="requestCallback"></param>
        /// <param name="cancellationToken"></param>
        public async Task<int> Run(RequestCallback requestCallback, CancellationToken cancellationToken = default)
        {
            var requestCount = _requestCount;

            var tasks = new List<Task<int>>(requestCount);

            try
            {
                for (var i = 0; i < requestCount; i++)
                    tasks.Add(RunRequest(requestCallback, cancellationToken));
            }
            catch (Exception)
            {
                if (tasks.Count == 0)
                    throw;
            }

            var counts = await Task.WhenAll(tasks).ConfigureAwait(false);

            return counts.Sum();
        }

        async Task<int> RunRequest(RequestCallback requestCallback, CancellationToken cancellationToken = default)
        {
            using var activeRequest = await BeginRequest(cancellationToken).ConfigureAwait(false);

            var count = await requestCallback(activeRequest.ResultLimit, activeRequest.CancellationToken).ConfigureAwait(false);

            await activeRequest.Complete(count, CancellationToken.None).ConfigureAwait(false);

            return count;
        }

        /// <summary>
        /// Run a series of requests, up the limits, as a single pass
        /// </summary>
        /// <param name="requestCallback"></param>
        /// <param name="resultCallback"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        public async Task<int> Run<T>(RequestCallback<T> requestCallback, ResultCallback<T> resultCallback, CancellationToken cancellationToken = default)
        {
            var requestCount = _requestCount;

            var tasks = new List<Task<int>>(requestCount);

            try
            {
                for (var i = 0; i < requestCount; i++)
                    tasks.Add(RunRequest(requestCallback, resultCallback, cancellationToken));
            }
            catch (Exception)
            {
                if (tasks.Count == 0)
                    throw;
            }

            var counts = await Task.WhenAll(tasks).ConfigureAwait(false);

            return counts.Sum();
        }

        async Task<int> RunRequest<T>(RequestCallback<T> requestCallback, ResultCallback<T> resultCallback, CancellationToken cancellationToken = default)
        {
            using var activeRequest = await BeginRequest(cancellationToken).ConfigureAwait(false);

            IEnumerable<T> results = await requestCallback(activeRequest.ResultLimit, activeRequest.CancellationToken).ConfigureAwait(false);

            var count = 0;
            try
            {
                foreach (var result in results)
                {
                    await _resultSemaphore.WaitAsync(activeRequest.CancellationToken).ConfigureAwait(false);

                    async Task RunResultCallback()
                    {
                        try
                        {
                            await resultCallback(result, activeRequest.CancellationToken).ConfigureAwait(false);
                        }
                        finally
                        {
                            if (!_disposed)
                                _resultSemaphore.Release();
                        }
                    }

                    Add(RunResultCallback());
                    count++;
                }
            }
            catch (Exception)
            {
                if (count == 0)
                    throw;
            }

            await activeRequest.Complete(count, CancellationToken.None).ConfigureAwait(false);

            return count;
        }

        /// <summary>
        /// Run a series of requests, up the limits, as a single pass
        /// </summary>
        /// <param name="requestCallback"></param>
        /// <param name="resultCallback"></param>
        /// <param name="groupCallback"></param>
        /// <param name="orderCallback"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        public async Task<int> Run<T, TKey>(RequestCallback<T> requestCallback, ResultCallback<T> resultCallback, GroupCallback<T, TKey> groupCallback,
            OrderCallback<T> orderCallback, CancellationToken cancellationToken = default)
        {
            var requestCount = _requestCount;

            var tasks = new List<Task<IReadOnlyList<T>>>(requestCount);

            try
            {
                for (var i = 0; i < requestCount; i++)
                    tasks.Add(RunRequest(requestCallback, cancellationToken));
            }
            catch (Exception)
            {
                if (tasks.Count == 0)
                    throw;
            }

            IReadOnlyList<T>[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

            List<IGrouping<TKey, T>> resultSets = groupCallback(results.SelectMany(x => x)).ToList();

            var resultTasks = new List<Task<int>>(ResultLimit);

            try
            {
                foreach (IGrouping<TKey, T> result in resultSets)
                    resultTasks.Add(RunResultSet(result, resultCallback, orderCallback, cancellationToken));
            }
            catch (Exception)
            {
                if (resultTasks.Count == 0)
                    throw;
            }

            var counts = await Task.WhenAll(resultTasks).ConfigureAwait(false);

            return counts.Sum();
        }

        async Task<IReadOnlyList<T>> RunRequest<T>(RequestCallback<T> requestCallback, CancellationToken cancellationToken = default)
        {
            using var activeRequest = await BeginRequest(cancellationToken).ConfigureAwait(false);

            List<T> results = (await requestCallback(activeRequest.ResultLimit, activeRequest.CancellationToken).ConfigureAwait(false)).ToList();

            await activeRequest.Complete(results.Count, CancellationToken.None).ConfigureAwait(false);

            return results;
        }

        async Task<int> RunResultSet<TKey, T>(IGrouping<TKey, T> results, ResultCallback<T> resultCallback, OrderCallback<T> orderCallback,
            CancellationToken cancellationToken = default)
        {
            var count = 0;

            try
            {
                foreach (var result in orderCallback(results))
                {
                    await _resultSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                    async Task RunResultCallback()
                    {
                        try
                        {
                            await resultCallback(result, cancellationToken).ConfigureAwait(false);
                        }
                        finally
                        {
                            _resultSemaphore.Release();
                        }
                    }

                    Add(RunResultCallback());
                    count++;
                }
            }
            catch (Exception)
            {
                if (count == 0)
                    throw;
            }

            return count;
        }

        public async Task<ActiveRequest> BeginRequest(CancellationToken cancellationToken = default)
        {
            try
            {
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeToken.Token);

                await _requestSemaphore.WaitAsync(linked.Token).ConfigureAwait(false);

                if (_rateLimitSemaphore != null)
                {
                    await _rateLimitSemaphore.WaitAsync(linked.Token).ConfigureAwait(false);

                    Interlocked.Increment(ref _count);
                }

                var current = Interlocked.Increment(ref _activeRequestCount);
                while (current > _maxRequestCount)
                    Interlocked.CompareExchange(ref _maxRequestCount, current, _maxRequestCount);

                int resultLimit;
                lock (_requestLock)
                {
                    resultLimit = Math.Min(_concurrentResultLimit - ActiveResultCount - _pendingResultCount, ResultLimit);
                    while (resultLimit < _refreshThreshold)
                    {
                        Monitor.Wait(_requestLock, 100);

                        if (cancellationToken.IsCancellationRequested)
                            cancellationToken.ThrowIfCancellationRequested();

                        resultLimit = Math.Min(_concurrentResultLimit - ActiveResultCount - _pendingResultCount, ResultLimit);
                    }

                    _pendingResultCount += resultLimit;
                }

                return new ActiveRequest(this, resultLimit, cancellationToken, _requestCancellationTimeout);
            }
            catch (OperationCanceledException)
            {
                if (!_disposed)
                    _requestSemaphore.Release();

                throw;
            }
        }

        internal Task EndRequest(int count, int resultLimit, CancellationToken cancellationToken = default)
        {
            Interlocked.Decrement(ref _activeRequestCount);

            lock (_requestLock)
            {
                _pendingResultCount -= resultLimit;

                Monitor.PulseAll(_requestLock);
            }

            if (_disposed)
                return Task.CompletedTask;

            _requestSemaphore.Release();

            var currentRequestCount = _requestCount;

            var requestCount = count >= _options.RequestResultLimit
                ? Math.Min(_requestLimit, currentRequestCount + (_requestLimit - currentRequestCount + 1) / 2)
                : Math.Max(1, currentRequestCount - currentRequestCount / 2);

            if (requestCount != currentRequestCount)
            {
                var previousValue = Interlocked.CompareExchange(ref _requestCount, requestCount, currentRequestCount);

                if (previousValue == currentRequestCount)
                    return ChangeRequestCount(requestCount, currentRequestCount, cancellationToken);
            }

            return Task.CompletedTask;
        }

        internal void CancelRequest(int resultLimit)
        {
            Interlocked.Decrement(ref _activeRequestCount);

            lock (_requestLock)
            {
                _pendingResultCount -= resultLimit;

                Monitor.PulseAll(_requestLock);
            }

            if (_disposed)
                return;

            _requestSemaphore.Release();
        }

        public async Task ChangeRateLimit(int newRateLimit, CancellationToken cancellationToken = default)
        {
            if (newRateLimit < 1)
                throw new ArgumentOutOfRangeException(nameof(newRateLimit), "The rate limit must be >= 1");

            if (_rateLimitSemaphore == null)
                throw new InvalidOperationException("Rate limit can only be changed when an original rate limit was specified.");

            if (_disposed)
                throw new ObjectDisposedException("The RequestRateAlgorithm was disposed");

            var previousLimit = _rateLimit;
            if (newRateLimit > previousLimit)
            {
                var releaseCount = newRateLimit - previousLimit;

                _rateLimitSemaphore.Release(releaseCount);

                Interlocked.Add(ref _rateLimit, releaseCount);
            }
            else
            {
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeToken.Token);

                for (; previousLimit > newRateLimit; previousLimit--)
                {
                    await _rateLimitSemaphore.WaitAsync(linked.Token).ConfigureAwait(false);

                    Interlocked.Decrement(ref _rateLimit);
                }
            }
        }

        async Task ChangeRequestCount(int newRequestCount, int currentRequestCount, CancellationToken cancellationToken = default)
        {
            if (newRequestCount < 1 || newRequestCount > _requestLimit)
                throw new ArgumentOutOfRangeException(nameof(newRequestCount), $"The request count {newRequestCount} must be >= 1 and <= {_requestLimit}");

            var previousRequestCount = currentRequestCount;
            if (newRequestCount > previousRequestCount)
            {
                var releaseCount = newRequestCount - previousRequestCount;

                _requestSemaphore.Release(releaseCount);

                Interlocked.Add(ref _rateLimit, releaseCount);
            }
            else
            {
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeToken.Token);

                for (; previousRequestCount > newRequestCount; previousRequestCount--)
                {
                    await _requestSemaphore.WaitAsync(linked.Token).ConfigureAwait(false);

                    Interlocked.Decrement(ref _rateLimit);
                }
            }
        }

        void Reset(object? state)
        {
            var processed = Interlocked.Exchange(ref _count, 0);
            if (processed > 0)
                _rateLimitSemaphore!.Release(processed);
        }

        void Add(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.Status == TaskStatus.RanToCompletion)
                return;

            var id = Interlocked.Increment(ref _nextId);

            lock (_tasks)
                _tasks.Add(id, task);

            task.ContinueWith(x => Remove(id), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
        }

        void Remove(long id)
        {
            int remaining;
            lock (_tasks)
            {
                _tasks.Remove(id);

                remaining = _tasks.Count;
            }

            if (remaining > 0)
                return;

            lock (_requestLock)
                Monitor.PulseAll(_requestLock);
        }
    }
}
