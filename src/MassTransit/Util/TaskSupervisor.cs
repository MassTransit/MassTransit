// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// An asynchronous signaling component
    /// </summary>
    public class TaskSupervisor :
        IDisposable,
        ITaskSupervisor
    {
        readonly List<Participant> _participants;
        readonly TaskCompletionSource<IStopEvent> _stopping;
        readonly CancellationTokenSource _stopToken;
        CancellationTokenRegistration _registration;

        public TaskSupervisor(CancellationToken cancellationToken)
            : this()
        {
            _registration = cancellationToken.Register(RaiseStopEventOnCancel);
        }

        TaskSupervisor(ITaskParticipant participant)
            : this()
        {
            OnStopRequested(participant.StopRequested);
        }

        public TaskSupervisor()
        {
            _stopping = new TaskCompletionSource<IStopEvent>();
            _participants = new List<Participant>();
            _stopToken = new CancellationTokenSource();
        }

        public CancellationToken StopToken => _stopToken.Token;

        public void Dispose()
        {
            _registration.Dispose();
            // _stopToken.Dispose(); this might make for some nasty disposed exceptions
        }

        public Task Ready
        {
            get
            {
                lock (_participants)
                    return Task.WhenAll(_participants.Select(x => x.ParticipantReady));
            }
        }

        public Task Completed
        {
            get
            {
                lock (_participants)
                    return Task.WhenAll(_participants.Select(x => x.ParticipantCompleted));
            }
        }

        public ITaskParticipant CreateParticipant()
        {
            var participant = new Participant();
            lock (_participants)
            {
                _participants.Add(participant);
            }

            return participant;
        }

        public ITaskSupervisorScope CreateScope()
        {
            var scope = new Scope();
            lock (_participants)
            {
                _participants.Add(scope);
            }

            return scope;
        }

        async void OnStopRequested(Task stopRequested)
        {
            await Task.Yield();

            await stopRequested.ConfigureAwait(false);

            await Stop("Parent Stop Requested").ConfigureAwait(false);
        }

        async void RaiseStopEventOnCancel()
        {
            await Task.Yield();

            await Stop("Parent Cancellation Requested").ConfigureAwait(false);
        }

        public async Task Stop(string reason)
        {
            var eventArgs = new StopEventArgs(reason);

            lock (_participants)
            {
                foreach (var participant in _participants)
                {
                    participant.Stop(eventArgs);
                }
            }

            await Completed.ConfigureAwait(false);

            _stopping.TrySetResult(eventArgs);
            _stopToken.Cancel();
        }


        class Scope :
            Participant,
            ITaskSupervisorScope
        {
            readonly TaskSupervisor _supervisor;

            public Scope()
            {
                _supervisor = new TaskSupervisor(this);
            }

            Task ITaskSupervisor.Ready => _supervisor.Ready;

            Task ITaskSupervisor.Completed => _supervisor.Completed;

            ITaskParticipant ITaskSupervisor.CreateParticipant()
            {
                return _supervisor.CreateParticipant();
            }

            ITaskSupervisorScope ITaskSupervisor.CreateScope()
            {
                return _supervisor.CreateScope();
            }
        }


        class Participant :
            ITaskParticipant
        {
            readonly TaskCompletionSource<bool> _complete;
            readonly TaskCompletionSource<bool> _ready;
            readonly TaskCompletionSource<IStopEvent> _stopping;
            readonly CancellationTokenSource _stopToken;

            public Participant()
            {
                _ready = new TaskCompletionSource<bool>();
                _complete = new TaskCompletionSource<bool>();
                _stopToken = new CancellationTokenSource();
                _stopping = new TaskCompletionSource<IStopEvent>();
            }

            public Task ParticipantReady => _ready.Task;
            public Task ParticipantCompleted => _complete.Task;

            Task ITaskParticipant.StopRequested => _stopping.Task;
            public CancellationToken StopToken => _stopToken.Token;

            public void SetComplete()
            {
                _complete.TrySetResult(true);
            }

            public void SetReady()
            {
                _ready.TrySetResult(true);
            }

            public void SetNotReady(Exception exception)
            {
                _ready.TrySetException(exception);
            }

            public void Dispose()
            {
                SetReady();
                SetComplete();
            }

            public void Stop(IStopEvent stopEvent)
            {
                _stopping.TrySetResult(stopEvent);
                _stopToken.Cancel();
            }
        }
    }
}