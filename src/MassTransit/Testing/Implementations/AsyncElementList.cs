namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;


    public abstract class AsyncElementList<TElement> :
        IAsyncElementList<TElement>
        where TElement : class, IAsyncListElement
    {
        readonly IList<Channel<TElement>> _listeners;
        readonly IList<TElement> _messages;
        readonly TimeSpan _timeout;
        CancellationToken _testCompleted;

        protected AsyncElementList(TimeSpan timeout, CancellationToken testCompleted = default)
        {
            _timeout = timeout;
            _testCompleted = testCompleted;
            _messages = new List<TElement>();
            _listeners = new List<Channel<TElement>>();
        }

        public async IAsyncEnumerable<TElement> SelectAsync(FilterDelegate<TElement> filter,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Channel<TElement> channel;
            lock (_messages)
            {
                foreach (var entry in _messages)
                {
                    if (filter(entry))
                        yield return entry;
                }

                if (cancellationToken.IsCancellationRequested || _testCompleted.IsCancellationRequested)
                    yield break;

                channel = Channel.CreateUnbounded<TElement>(new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false,
                    AllowSynchronousContinuations = false
                });

                lock (_listeners)
                    _listeners.Add(channel);
            }

            var timeoutTokenSource = new CancellationTokenSource(_timeout);

            CancellationTokenRegistration cancellationTokenRegistration = default;
            if (cancellationToken.CanBeCanceled)
                cancellationTokenRegistration = cancellationToken.Register(timeoutTokenSource.Cancel);

            CancellationTokenRegistration testCompletedTokenRegistration = default;
            if (_testCompleted.CanBeCanceled)
                testCompletedTokenRegistration = _testCompleted.Register(timeoutTokenSource.Cancel);

            try
            {
                var more = true;
                while (more)
                {
                    try
                    {
                        more = await channel.Reader.WaitToReadAsync(timeoutTokenSource.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        more = false;
                    }

                    if (more)
                    {
                        if (channel.Reader.TryRead(out var entry) && filter(entry))
                            yield return entry;
                    }
                }
            }
            finally
            {
                cancellationTokenRegistration.Dispose();
                testCompletedTokenRegistration.Dispose();

                timeoutTokenSource.Dispose();
            }
        }

        public async Task<bool> Any(FilterDelegate<TElement> filter, CancellationToken cancellationToken = default)
        {
            try
            {
                await foreach (var _ in SelectAsync(filter, cancellationToken).ConfigureAwait(false))
                    return true;
            }
            catch (OperationCanceledException)
            {
            }

            return false;
        }

        public IEnumerable<TElement> Select(FilterDelegate<TElement> filter, CancellationToken cancellationToken = default)
        {
            lock (_messages)
            {
                var index = 0;
                for (; index < _messages.Count; index++)
                {
                    var entry = _messages[index];
                    if (filter(entry))
                        yield return entry;
                }

                if (cancellationToken.IsCancellationRequested || _testCompleted.IsCancellationRequested)
                    yield break;

                var monitorTimeout = _timeout;
                var endTime = DateTime.Now + monitorTimeout;

                void Cancel()
                {
                    endTime = DateTime.Now;

                    lock (_messages)
                        Monitor.PulseAll(_messages);
                }

                CancellationTokenRegistration cancellationTokenRegistration = default;
                if (cancellationToken.CanBeCanceled)
                    cancellationTokenRegistration = cancellationToken.Register(() => Cancel());

                CancellationTokenRegistration timeoutTokenRegistration = default;
                if (_testCompleted.CanBeCanceled)
                    timeoutTokenRegistration = _testCompleted.Register(() => Cancel());

                try
                {
                    while (Monitor.Wait(_messages, monitorTimeout))
                    {
                        for (; index < _messages.Count; index++)
                        {
                            var element = _messages[index];
                            if (filter(element))
                                yield return element;
                        }

                        monitorTimeout = endTime - DateTime.Now;
                        if (monitorTimeout <= TimeSpan.Zero)
                            break;

                        if (cancellationToken.IsCancellationRequested || _testCompleted.IsCancellationRequested)
                            yield break;
                    }
                }
                finally
                {
                    cancellationTokenRegistration.Dispose();
                    timeoutTokenRegistration.Dispose();
                }
            }
        }

        protected void Add(TElement context)
        {
            lock (_messages)
            {
                if (_messages.Any(x => x.ElementId == context.ElementId))
                    return;

                _messages.Add(context);

                Monitor.PulseAll(_messages);

                lock (_listeners)
                {
                    foreach (Channel<TElement> channel in _listeners)
                        channel.Writer.TryWrite(context);
                }
            }
        }
    }
}
