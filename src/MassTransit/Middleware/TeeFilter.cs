namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// Connects multiple output pipes to a single input pipe
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class TeeFilter<TContext> :
        ITeeFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly Connectable<IPipe<TContext>> _connections;

        public TeeFilter()
        {
            _connections = new Connectable<IPipe<TContext>>();
        }

        public int Count => _connections.Count;

        void IProbeSite.Probe(ProbeContext context)
        {
            _connections.ForEach(pipe => pipe.Probe(context));
        }

        [DebuggerNonUserCode]
        public Task Send(TContext context, IPipe<TContext> next)
        {
            var connectionsTask = _connections.ForEachAsync(pipe => pipe.Send(context));
            if (connectionsTask.Status == TaskStatus.RanToCompletion)
                return next.Send(context);

            async Task SendAsync()
            {
                await connectionsTask.ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }

            return SendAsync();
        }

        public ConnectHandle ConnectPipe(IPipe<TContext> pipe)
        {
            return _connections.Connect(pipe);
        }
    }


    /// <summary>
    /// Connects multiple output pipes to a single input pipe
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TKey">The key type</typeparam>
    public class TeeFilter<TContext, TKey> :
        TeeFilter<TContext>,
        ITeeFilter<TContext, TKey>
        where TContext : class, PipeContext
    {
        readonly KeyAccessor<TContext, TKey> _keyAccessor;
        readonly Lazy<IKeyPipeConnector<TKey>> _keyConnections;

        public TeeFilter(KeyAccessor<TContext, TKey> keyAccessor)
        {
            _keyAccessor = keyAccessor;

            _keyConnections = new Lazy<IKeyPipeConnector<TKey>>(ConnectKeyFilter);
        }

        public ConnectHandle ConnectPipe<T>(TKey key, IPipe<T> pipe)
            where T : class, PipeContext
        {
            return _keyConnections.Value.ConnectPipe(key, pipe);
        }

        IKeyPipeConnector<TKey> ConnectKeyFilter()
        {
            var filter = new KeyFilter<TContext, TKey>(_keyAccessor);

            ConnectPipe(filter.ToPipe());

            return filter;
        }
    }
}
