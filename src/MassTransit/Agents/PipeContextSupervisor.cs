namespace MassTransit.Agents
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using Middleware;


    /// <summary>
    /// Maintains a cached context, which is created upon first use, and recreated whenever a fault is propagated to the usage.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class PipeContextSupervisor<TContext> :
        Supervisor,
        ISupervisor<TContext>
        where TContext : class, PipeContext
    {
        readonly ISupervisor _activeSupervisor;
        readonly IPipeContextFactory<TContext> _contextFactory;
        readonly object _contextLock = new object();
        PipeContextHandle<TContext> _context;

        /// <summary>
        /// Create the cache
        /// </summary>
        /// <param name="contextFactory">Factory used to create the underlying and active contexts</param>
        public PipeContextSupervisor(IPipeContextFactory<TContext> contextFactory)
        {
            _contextFactory = contextFactory;

            _activeSupervisor = new Supervisor();
        }

        protected bool HasContext
        {
            get
            {
                lock (_contextLock)
                {
                    return _context is { IsDisposed: false };
                }
            }
        }

        async Task IPipeContextSource<TContext>.Send(IPipe<TContext> pipe, CancellationToken cancellationToken)
        {
            IActivePipeContextAgent<TContext> activeContext = CreateActiveContext(cancellationToken);

            try
            {
                var context = activeContext.Context.Status == TaskStatus.RanToCompletion
                    ? activeContext.Context.Result
                    : await activeContext.Context.ConfigureAwait(false);

                await pipe.Send(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await activeContext.Faulted(exception).ConfigureAwait(false);

                throw;
            }
            finally
            {
                await activeContext.Stop(cancellationToken).ConfigureAwait(false);

                await activeContext.DisposeAsync().ConfigureAwait(false);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("source");
            scope.Set(new
            {
                Type = TypeCache<PipeContextSupervisor<TContext>>.ShortName,
                HasContext,
            });
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            SetCompleted(ActiveAndActualAgentsCompleted(context));

            // stop the active context agents
            await _activeSupervisor.Stop(context).ConfigureAwait(false);

            await Task.WhenAll(context.Agents.Select(x => x.Stop(context))).OrCanceled(context.CancellationToken).ConfigureAwait(false);

            await Completed.OrCanceled(context.CancellationToken).ConfigureAwait(false);
        }

        async Task ActiveAndActualAgentsCompleted(StopSupervisorContext context)
        {
            await _activeSupervisor.Completed.ConfigureAwait(false);

            await Task.WhenAll(context.Agents.Select(x => x.Completed)).ConfigureAwait(false);
        }

        IActivePipeContextAgent<TContext> CreateActiveContext(CancellationToken cancellationToken)
        {
            PipeContextHandle<TContext> pipeContextHandle = GetContext();

            return _contextFactory.CreateActiveContext(_activeSupervisor, pipeContextHandle, cancellationToken);
        }

        PipeContextHandle<TContext> GetContext()
        {
            lock (_contextLock)
            {
                if (_context is { IsDisposed: false })
                    return _context;

                PipeContextHandle<TContext> context = _context = _contextFactory.CreateContext(this);

                void ClearContext(Task task)
                {
                    Interlocked.CompareExchange(ref _context, null, context);
                }

                context.Context.ContinueWith(ClearContext, CancellationToken.None, TaskContinuationOptions.NotOnRanToCompletion, TaskScheduler.Default);

                SetReady(context.Context);

                return context;
            }
        }
    }
}
