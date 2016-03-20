// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Extensions;


    /// <summary>
    /// An asynchronous signaling component
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public class TaskSupervisor :
        ITaskSupervisor
    {
        readonly Dictionary<long, ITaskParticipant> _participants; 
        readonly CancellationTokenSource _stoppedToken;
        readonly CancellationTokenSource _stoppingToken;
        readonly TaskCompletionSource<IStopEvent> _stopRequested;
        readonly string _tag;
        long _nextId;

        public TaskSupervisor(string tag, ITaskParticipant participant)
            : this(tag)
        {
            OnStopRequested(participant.StopRequested);
        }

        public TaskSupervisor(string tag)
        {
            _tag = tag;
            _participants = new Dictionary<long, ITaskParticipant>();

            _stopRequested = new TaskCompletionSource<IStopEvent>();

            _stoppingToken = new CancellationTokenSource();
            _stoppedToken = new CancellationTokenSource();
        }

        public CancellationToken StoppedToken => _stoppedToken.Token;
        public CancellationToken StoppingToken => _stoppingToken.Token;

        public Task<IStopEvent> StopRequested => _stopRequested.Task;

        public Task Ready
        {
            get
            {
                lock (_participants)
                    return Task.WhenAll(_participants.Select(x => x.Value.ParticipantReady).ToArray());
            }
        }

        public Task Completed
        {
            get
            {
                lock (_participants)
                    return Task.WhenAll(_participants.Select(x => x.Value.ParticipantCompleted).ToArray());
            }
        }

        public ITaskParticipant CreateParticipant(string tag)
        {
            if (_stoppingToken.IsCancellationRequested)
                throw new OperationCanceledException("The supervisor is stopping, no additional participants can be created");

            long id = Interlocked.Increment(ref _nextId);

            var participant = new TaskParticipant(tag, () => RemoveParticipant(id));
            lock (_participants)
            {
                _participants.Add(id, participant);
            }

            return participant;
        }

        void RemoveParticipant(long id)
        {
            lock (_participants)
                _participants.Remove(id);
        }

        public ITaskScope CreateScope(string tag)
        {
            if (_stoppingToken.IsCancellationRequested)
                throw new OperationCanceledException("The supervisor is stopping, no additional scopes can be created");

            long id = Interlocked.Increment(ref _nextId);

            var scope = new TaskScope(tag, () => RemoveParticipant(id));
            lock (_participants)
            {
                _participants.Add(id, scope);
            }

            return scope;
        }

        public string Tag => _tag;

        async Task WhenAll(IEnumerable<ITaskParticipant> participants, Func<ITaskParticipant, Task> selector)
        {
            ITaskParticipant[] taskArray = participants.ToArray();

            do
            {
                var delayTask = Task.Delay(1000);

                var readyTask = await Task.WhenAny(taskArray.Select(selector).Concat(Enumerable.Repeat(delayTask, 1))).ConfigureAwait(false);
                if (delayTask == readyTask)
                {
                    Console.WriteLine();
                    Console.WriteLine("Waiting: {0}", ToString());
                    foreach (var task in taskArray)
                    {
                        Console.WriteLine("{0} - {1}", task, selector(task).Status);
                    }

                }
                else
                {
                    var ready = taskArray.Where(x => selector(x).Status == TaskStatus.RanToCompletion || selector(x).Status == TaskStatus.Canceled);
                    foreach (var participant in ready)
                    {
                        Console.WriteLine("Completed: {0} - {1}", participant, ToString());
                    }

                    taskArray = taskArray.Where(x => selector(x).Status != TaskStatus.RanToCompletion && selector(x).Status != TaskStatus.Canceled)
                        .ToArray();
                }
            }
            while (taskArray.Length > 0);
        }

        void OnStopRequested(Task stopRequested)
        {
            stopRequested.ContinueWith(async stopTask =>
            {
                await stopTask.ConfigureAwait(false);

                await Stop("Parent Stop Requested").ConfigureAwait(false);
            });
        }

        public async Task Stop(string reason, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventArgs = new StopEventArgs(reason);
            _stoppingToken.Cancel();
            _stopRequested.TrySetResult(eventArgs);

            lock (_participants)
            {
                foreach (var participant in _participants)
                {
                    participant.Value.Stop(eventArgs);
                }
            }

            await Completed.WithCancellation(cancellationToken).ConfigureAwait(false);

            _stoppedToken.Cancel();
        }

        public override string ToString()
        {
            return $"Supervisor: {_tag}";
        }

        string DebuggerDisplay()
        {
            return $"Supervisor: {_tag} - Participants: {_participants.Count}";
        }
    }
}