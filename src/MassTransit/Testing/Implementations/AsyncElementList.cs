namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Util;


    public abstract class AsyncElementList<TElement> :
        IAsyncElementList<TElement>
        where TElement : class, IAsyncListElement
    {
        readonly Connectable<Channel<TElement>> _channels;
        readonly IList<TElement> _messages;
        readonly TimeSpan _timeout;
        CancellationToken _testCompleted;

        protected AsyncElementList(TimeSpan timeout, CancellationToken testCompleted = default)
        {
            _timeout = timeout;
            _testCompleted = testCompleted;

            _messages = new List<TElement>();
            _channels = new Connectable<Channel<TElement>>();
        }

        public async IAsyncEnumerable<TElement> SelectAsync(FilterDelegate<TElement> filter,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var channel = Channel.CreateUnbounded<TElement>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            });

            var handle = _channels.Connect(channel);

            var returned = new HashSet<Guid>();

            try
            {
                TElement[] messages;
                lock (_messages)
                    messages = _messages.ToArray();

                foreach (var entry in messages)
                {
                    if (filter(entry) && !returned.Contains(entry.ElementId.Value))
                    {
                        returned.Add(entry.ElementId.Value);
                        yield return entry;
                    }
                }

                if (cancellationToken.IsCancellationRequested || _testCompleted.IsCancellationRequested)
                    yield break;

                using var timeout = new CancellationTokenSource(_timeout);

                using var linked = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, _testCompleted, cancellationToken);

                while (!linked.IsCancellationRequested)
                {
                    try
                    {
                        await channel.Reader.WaitToReadAsync(linked.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    if (!channel.Reader.TryRead(out _))
                        break;

                    lock (_messages)
                        messages = _messages.ToArray();

                    foreach (var entry in messages)
                    {
                        if (filter(entry) && !returned.Contains(entry.ElementId.Value))
                        {
                            returned.Add(entry.ElementId.Value);
                            yield return entry;
                        }
                    }
                }
            }
            finally
            {
                handle.Disconnect();
                channel.Writer.Complete();
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
            if (!context.ElementId.HasValue)
                return;

            lock (_messages)
            {
                if (_messages.Any(x => x.ElementId == context.ElementId))
                    return;

                _messages.Add(context);

                Monitor.PulseAll(_messages);
            }

            _channels.ForEach(channel => channel.Writer.TryWrite(context));
        }
    }
}
