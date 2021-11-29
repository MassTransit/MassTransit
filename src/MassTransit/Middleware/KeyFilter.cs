namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;


    /// <summary>
    /// Handles the registration of requests and connecting them to the consume pipe
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class KeyFilter<TContext, TKey> :
        IFilter<TContext>,
        IKeyPipeConnector<TKey>
        where TContext : class, PipeContext
    {
        readonly KeyAccessor<TContext, TKey> _keyAccessor;
        readonly ConcurrentDictionary<TKey, IPipe<TContext>> _pipes;

        public KeyFilter(KeyAccessor<TContext, TKey> keyAccessor)
        {
            _keyAccessor = keyAccessor;
            _pipes = new ConcurrentDictionary<TKey, IPipe<TContext>>();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("key");

            ICollection<IPipe<TContext>> pipes = _pipes.Values;
            scope.Add("count", pipes.Count);

            foreach (IPipe<TContext> pipe in pipes)
                pipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        public async Task Send(TContext context, IPipe<TContext> next)
        {
            var key = _keyAccessor(context);

            if (_pipes.TryGetValue(key, out IPipe<TContext> pipe))
                await pipe.Send(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public ConnectHandle ConnectPipe<T>(TKey key, IPipe<T> pipe)
            where T : class, PipeContext
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            if (pipe is IPipe<TContext> keyPipe)
            {
                var added = _pipes.TryAdd(key, keyPipe);
                if (!added)
                    throw new DuplicateKeyPipeConfigurationException($"A pipe with the specified key already exists: {key}");

                return new Handle(key, RemovePipe);
            }

            throw new ArgumentException($"The pipe must match the input type: {TypeCache<TContext>.ShortName}", nameof(pipe));
        }

        void RemovePipe(TKey key)
        {
            _pipes.TryRemove(key, out IPipe<TContext> _);
        }


        class Handle :
            ConnectHandle
        {
            readonly TKey _key;
            readonly Action<TKey> _removeKey;

            public Handle(TKey key, Action<TKey> removeKey)
            {
                _key = key;
                _removeKey = removeKey;
            }

            public void Disconnect()
            {
                _removeKey(_key);
            }

            public void Dispose()
            {
                Disconnect();
            }
        }
    }
}
