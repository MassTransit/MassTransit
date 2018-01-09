// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace GreenPipes.Agents
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Internals.Extensions;
    using Util;


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

                if (!IsAlreadyReady)
                    SetReady(Task.WhenAll(_agents.Values.Select(x => x.Ready).ToArray()));
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
                lock (_agents)
                {
                    if (_agents.Count == 0)
                        SetReady(TaskUtil.Completed);
                    else
                        SetReady(Task.WhenAll(_agents.Values.Select(x => x.Ready).ToArray()));
                }
        }

        /// <inheritdoc />
        protected override Task StopAgent(StopContext context)
        {
            IAgent[] agents;
            lock (_agents)
                agents = _agents.Values.Where(x => !x.Completed.IsCompleted).ToArray();

            return StopSupervisor(new Context(context, agents));
        }

        protected virtual async Task StopSupervisor(StopSupervisorContext context)
        {
            SetCompleted(Task.WhenAll(context.Agents.Select(x => x.Completed)));

            await Task.WhenAll(context.Agents.Select(x => x.Stop(context))).UntilCompletedOrCanceled(context.CancellationToken).ConfigureAwait(false);

            await Completed.ConfigureAwait(false);
        }

        void Remove(long id)
        {
            lock (_agents)
            {
                _agents.Remove(id);
            }
        }

        Task WhenAll(IAgent[] agents, string readyOrCompleted, Func<IAgent, Task> selector)
        {
            if (Trace.Listeners.Count == 0)
                return Task.WhenAll(agents.Select(selector).ToArray());

            async Task WaitForAll()
            {
                await Task.Yield();

                List<Task> faultedTasks = new List<Task>();
                do
                {
                    var delayTask = Task.Delay(1000);

                    var readyTask = await Task.WhenAny(agents.Select(selector).Concat(Enumerable.Repeat(delayTask, 1))).ConfigureAwait(false);
                    if (delayTask == readyTask)
                    {
                        Trace.WriteLine($"Waiting: {ToString()}");
                        Trace.WriteLine(string.Join(Environment.NewLine, agents.Select(agent => $"{agent} - {selector(agent).Status}")));
                    }
                    else
                    {
                        Trace.WriteLine($"{readyOrCompleted} Updated: {ToString()}");
                        var completed = from agent in agents
                            let task = selector(agent)
                            where task.IsCompleted
                            select new {agent, task};

                        var completedAgents = completed.ToDictionary(x => x.agent);

                        foreach (var item in completedAgents.Values)
                            if (item.task.IsCanceled)
                            {
                                Trace.WriteLine($"Canceled: {item.agent}");
                            }
                            else if (item.task.IsFaulted)
                            {
                                Trace.WriteLine($"Faulted: {item.agent}");
                                faultedTasks.Add(item.task);
                            }
                            else
                            {
                                Trace.WriteLine($"{readyOrCompleted}: {item.agent}");
                            }

                        agents = agents.Where(x => !completedAgents.ContainsKey(x)).ToArray();
                    }
                }
                while (agents.Length > 0);

                if (faultedTasks.Count > 0)
                    await Task.WhenAll(faultedTasks).ConfigureAwait(false);
            }

            return WaitForAll();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Supervisor";
        }


        class Context :
            StopSupervisorContext
        {
            readonly StopContext _context;

            public Context(StopContext context, IAgent[] agents)
            {
                _context = context;
                Agents = agents;
            }

            bool PipeContext.HasPayloadType(Type payloadType)
            {
                return _context.HasPayloadType(payloadType);
            }

            bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
            {
                return _context.TryGetPayload(out payload);
            }

            TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            {
                return _context.GetOrAddPayload(payloadFactory);
            }

            CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

            string StopContext.Reason => _context.Reason;

            public IAgent[] Agents { get; }
        }
    }
}