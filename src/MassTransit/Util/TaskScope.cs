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
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;


    [DebuggerDisplay("{DebuggerDisplay()}")]
    class TaskScope :
        ITaskScope
    {
        readonly ITaskParticipant _participant;
        readonly TaskSupervisor _supervisor;

        public TaskScope(string tag, Action remove)
        {
            _participant = new TaskParticipant(tag, remove);

            _supervisor = new TaskSupervisor(tag, this);
        }

        Task ITaskSupervisor.Ready => _supervisor.Ready;

        Task ITaskSupervisor.Completed => _supervisor.Completed;

        CancellationToken ITaskParticipant.StoppingToken => _participant.StoppingToken;

        CancellationToken ITaskParticipant.StoppedToken => _participant.StoppedToken;

        void ITaskParticipant.SetComplete()
        {
            _participant.SetComplete();
        }

        void ITaskParticipant.SetReady()
        {
            _participant.SetReady();
        }

        void ITaskParticipant.SetNotReady(Exception exception)
        {
            _participant.SetNotReady(exception);
        }

        void ITaskParticipant.Stop(IStopEvent stopEvent)
        {
            _participant.Stop(stopEvent);
        }

        Task ITaskParticipant.ParticipantReady => _participant.ParticipantReady;

        Task ITaskParticipant.ParticipantCompleted => _participant.ParticipantCompleted;

        ITaskParticipant ITaskSupervisor.CreateParticipant(string tag)
        {
            return _supervisor.CreateParticipant(tag);
        }

        ITaskScope ITaskSupervisor.CreateScope(string tag)
        {
            return _supervisor.CreateScope(tag);
        }

        static readonly ILog _log = Logger.Get<TaskScope>();

        async Task ITaskScope.Stop(string reason, Func<Task> afterStopped, CancellationToken cancellationToken)
        {
            _participant.Stop(new StopEventArgs(reason));

            await _supervisor.Stop(reason, cancellationToken).ConfigureAwait(false);

            try
            {
                await afterStopped().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Exception after stopping scope {_supervisor.Tag}", ex);
            }

            _participant.SetComplete();
        }

        Task<IStopEvent> ITaskParticipant.StopRequested => _participant.StopRequested;

        void IDisposable.Dispose()
        {
            _participant.SetReady();

            _participant.Stop(new StopEventArgs("Participant Disposed"));

            _participant.SetComplete();
        }

        public override string ToString()
        {
            return $"Scope: {_supervisor.Tag}";
        }

        string DebuggerDisplay()
        {
            return $"Scope: {_supervisor.Tag}";
        }
    }
}