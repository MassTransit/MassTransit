namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;


    /// <summary>
    /// Supervises a set of agents, allowing for graceful Start, Stop, and Ready state management
    /// </summary>
    public class Supervisor :
        Agent,
        ISupervisor
    {
        readonly Dictionary<long, IAgent> _agents;
        long _nextId;
        int _peakActiveCount;
        long _totalCount;

        /// <summary>
        /// Creates a Supervisor
        /// </summary>
        public Supervisor()
        {
            _agents = new Dictionary<long, IAgent>();
        }

        /// <inheritdoc />
        public void Add(IAgent agent)
        {
            if (IsStopping)
                throw new OperationCanceledException("The agent is stopped or has been stopped, no additional provocateurs can be created.");

            var id = Interlocked.Increment(ref _nextId);
            lock (_agents)
            {
                _agents.Add(id, agent);

                _totalCount++;
                var currentActiveCount = _agents.Count;

                if (currentActiveCount > _peakActiveCount)
                    _peakActiveCount = currentActiveCount;

                SetReady();
            }

            void RemoveAgent(Task task)
            {
                Remove(id);
            }

            agent.Completed.ContinueWith(RemoveAgent, TaskScheduler.Default);
        }

        /// <inheritdoc />
        public int PeakActiveCount => _peakActiveCount;

        /// <inheritdoc />
        public long TotalCount => _totalCount;

        /// <inheritdoc />
        public override void SetReady()
        {
            if (!IsAlreadyReady)
            {
                lock (_agents)
                {
                    SetReady(_agents.Count == 0
                        ? Task.CompletedTask
                        : Task.WhenAll(_agents.Values.Select(x => x.Ready).ToArray()));
                }
            }
        }

        /// <inheritdoc />
        protected override Task StopAgent(StopContext context)
        {
            IAgent[] agents;
            lock (_agents)
            {
                agents = _agents.Count == 0
                    ? Array.Empty<IAgent>()
                    : _agents.Values.Where(x => !x.Completed.IsCompleted).ToArray();
            }

            return StopSupervisor(new Context(context, agents));
        }

        protected virtual async Task StopSupervisor(StopSupervisorContext context)
        {
            if (context.Agents.Length == 0)
                SetCompleted(Task.CompletedTask);

            if (context.Agents.Length == 1)
            {
                SetCompleted(context.Agents[0].Completed);

                await context.Agents[0].Stop(context).OrCanceled(context.CancellationToken).ConfigureAwait(false);
            }
            else if (context.Agents.Length > 1)
            {
                var completedTasks = new Task[context.Agents.Length];
                for (var i = 0; i < context.Agents.Length; i++)
                    completedTasks[i] = context.Agents[i].Completed;

                SetCompleted(Task.WhenAll(completedTasks));

                var stopTasks = new Task[context.Agents.Length];
                for (var i = 0; i < context.Agents.Length; i++)
                    stopTasks[i] = context.Agents[i].Stop(context);

                await Task.WhenAll(stopTasks).OrCanceled(context.CancellationToken).ConfigureAwait(false);
            }

            await Completed.OrCanceled(context.CancellationToken).ConfigureAwait(false);
        }

        void Remove(long id)
        {
            lock (_agents)
                _agents.Remove(id);
        }


        class Context :
            ProxyPipeContext,
            StopSupervisorContext
        {
            readonly StopContext _context;

            public Context(StopContext context, IAgent[] agents)
                : base(context)
            {
                _context = context;
                Agents = agents;
            }

            string StopContext.Reason => _context.Reason;

            public IAgent[] Agents { get; }
        }
    }
}
