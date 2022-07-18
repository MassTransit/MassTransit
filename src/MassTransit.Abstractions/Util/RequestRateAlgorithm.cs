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


        readonly RequestRateAlgorithmOptions _options;
        readonly SemaphoreSlim? _rateLimitSemaphore;
        readonly Timer? _rateLimitTimer;
        readonly SemaphoreSlim _requestCountSemaphore;
        readonly int _requestLimit;
        readonly int _resultLimit;

        int _activeRequestCount;

        int _count;
        int _maxRequestCount;
        int _rateLimit;
        int _requestCount;

        public RequestRateAlgorithm(RequestRateAlgorithmOptions options)
        {
            _options = options;

            _requestCount = 1;
            _requestLimit = (_options.PrefetchCount + _options.RequestResultLimit - 1) / _options.RequestResultLimit;
            _resultLimit = Math.Min(_options.PrefetchCount, _options.RequestResultLimit);

            _requestCountSemaphore = new SemaphoreSlim(_requestCount);

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

        public void Dispose()
        {
            _rateLimitTimer?.Dispose();
            _rateLimitSemaphore?.Dispose();

            _requestCountSemaphore.Dispose();
        }

        /// <summary>
        /// Run a series of requests, up the limits, as a single pass
        /// </summary>
        /// <param name="requestCallback"></param>
        /// <param name="cancellationToken"></param>
        public async Task Run(RequestCallback requestCallback, CancellationToken cancellationToken = default)
        {
            var requestCount = _requestCount;

            var tasks = new List<Task>(requestCount);

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

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        async Task RunRequest(RequestCallback requestCallback, CancellationToken cancellationToken = default)
        {
            using var activeRequest = await BeginRequest(cancellationToken).ConfigureAwait(false);

            var count = await requestCallback(ResultLimit, cancellationToken).ConfigureAwait(false);

            await activeRequest.Complete(count, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Run a series of requests, up the limits, as a single pass
        /// </summary>
        /// <param name="requestCallback"></param>
        /// <param name="resultCallback"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        public async Task Run<T>(RequestCallback<T> requestCallback, ResultCallback<T> resultCallback, CancellationToken cancellationToken = default)
        {
            var requestCount = _requestCount;

            var tasks = new List<Task>(requestCount);

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

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        async Task RunRequest<T>(RequestCallback<T> requestCallback, ResultCallback<T> resultCallback, CancellationToken cancellationToken = default)
        {
            using var activeRequest = await BeginRequest(cancellationToken).ConfigureAwait(false);

            IEnumerable<T> results = await requestCallback(ResultLimit, cancellationToken).ConfigureAwait(false);

            var tasks = new List<Task>(ResultLimit);

            try
            {
                foreach (var result in results)
                    tasks.Add(resultCallback(result, cancellationToken));
            }
            catch (Exception)
            {
                if (tasks.Count == 0)
                    throw;
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            await activeRequest.Complete(tasks.Count, CancellationToken.None).ConfigureAwait(false);
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
        public async Task Run<T, TKey>(RequestCallback<T> requestCallback, ResultCallback<T> resultCallback, GroupCallback<T, TKey> groupCallback,
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

            var resultTasks = new List<Task>(ResultLimit);

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

            await Task.WhenAll(resultTasks).ConfigureAwait(false);
        }

        async Task<IReadOnlyList<T>> RunRequest<T>(RequestCallback<T> requestCallback, CancellationToken cancellationToken = default)
        {
            using var activeRequest = await BeginRequest(cancellationToken).ConfigureAwait(false);

            List<T> results = (await requestCallback(ResultLimit, cancellationToken).ConfigureAwait(false)).ToList();

            await activeRequest.Complete(results.Count, CancellationToken.None).ConfigureAwait(false);

            return results;
        }

        async Task RunResultSet<TKey, T>(IGrouping<TKey, T> results, ResultCallback<T> resultCallback, OrderCallback<T> orderCallback,
            CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>(ResultLimit);

            try
            {
                foreach (var result in orderCallback(results))
                    tasks.Add(resultCallback(result, cancellationToken));
            }
            catch (Exception)
            {
                if (tasks.Count == 0)
                    throw;
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task<ActiveRequest> BeginRequest(CancellationToken cancellationToken = default)
        {
            try
            {
                await _requestCountSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                if (_rateLimitSemaphore != null)
                {
                    await _rateLimitSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                    Interlocked.Increment(ref _count);
                }

                var current = Interlocked.Increment(ref _activeRequestCount);
                while (current > _maxRequestCount)
                    Interlocked.CompareExchange(ref _maxRequestCount, current, _maxRequestCount);

                return new ActiveRequest(this);
            }
            catch (OperationCanceledException)
            {
                _requestCountSemaphore.Release();

                throw;
            }
        }

        internal Task EndRequest(int count, CancellationToken cancellationToken = default)
        {
            Interlocked.Decrement(ref _activeRequestCount);

            _requestCountSemaphore.Release();

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

        internal void CancelRequest()
        {
            Interlocked.Decrement(ref _activeRequestCount);

            _requestCountSemaphore.Release();
        }

        public async Task ChangeRateLimit(int newRateLimit, CancellationToken cancellationToken = default)
        {
            if (newRateLimit < 1)
                throw new ArgumentOutOfRangeException(nameof(newRateLimit), "The rate limit must be >= 1");

            if (_rateLimitSemaphore == null)
                throw new InvalidOperationException("Rate limit can only be changed when an original rate limit was specified.");

            var previousLimit = _rateLimit;
            if (newRateLimit > previousLimit)
            {
                var releaseCount = newRateLimit - previousLimit;

                _rateLimitSemaphore.Release(releaseCount);

                Interlocked.Add(ref _rateLimit, releaseCount);
            }
            else
            {
                for (; previousLimit > newRateLimit; previousLimit--)
                {
                    await _rateLimitSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

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

                _requestCountSemaphore.Release(releaseCount);

                Interlocked.Add(ref _rateLimit, releaseCount);
            }
            else
            {
                for (; previousRequestCount > newRequestCount; previousRequestCount--)
                {
                    await _requestCountSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

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
    }
}
