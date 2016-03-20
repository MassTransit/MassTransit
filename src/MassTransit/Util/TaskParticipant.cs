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


    [DebuggerDisplay("{DebuggerDisplay()}")]
    class TaskParticipant :
        ITaskParticipant
    {
        readonly TaskCompletionSource<bool> _completed;
        readonly TaskCompletionSource<bool> _ready;
        readonly CancellationTokenSource _stoppedToken;
        readonly CancellationTokenSource _stoppingToken;
        readonly TaskCompletionSource<IStopEvent> _stopRequested;
        readonly string _tag;
        readonly Action _remove;

        public TaskParticipant(string tag, Action remove)
        {
            _tag = tag;
            _remove = remove;
            _stoppingToken = new CancellationTokenSource();
            _stoppedToken = new CancellationTokenSource();

            _ready = new TaskCompletionSource<bool>();
            _completed = new TaskCompletionSource<bool>();
            _stopRequested = new TaskCompletionSource<IStopEvent>();
        }

        public Task ParticipantReady => _ready.Task;

        public Task ParticipantCompleted => _completed.Task;

        public Task<IStopEvent> StopRequested => _stopRequested.Task;

        public CancellationToken StoppingToken => _stoppingToken.Token;
        public CancellationToken StoppedToken => _stoppedToken.Token;

        public void SetComplete()
        {
            _stoppedToken.Cancel();
            _completed.TrySetResult(true);

            _remove();
        }

        public void SetReady()
        {
            _ready.TrySetResult(true);
        }

        public void SetNotReady(Exception exception)
        {
            _ready.TrySetException(exception);
        }

        void ITaskParticipant.Stop(IStopEvent stopEvent)
        {
            _stoppingToken.Cancel();
            _stopRequested.TrySetResult(stopEvent);
        }

        public override string ToString()
        {
            return $"Participant: {_tag}";
        }

        string DebuggerDisplay()
        {
            return $"Participant: {_tag}";
        }
    }
}