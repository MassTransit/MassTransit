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
namespace MassTransit.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes.Internals.Extensions;
    using Internals.Extensions;


    public class ConsumerJobHandle<T> :
        JobHandle<T>
        where T : class
    {
        readonly ConsumerJobContext<T> _jobContext;
        readonly Task _task;

        public ConsumerJobHandle(ConsumerJobContext<T> jobContext, Task task)
        {
            _jobContext = jobContext;
            _task = task;
        }

        public Guid JobId => _jobContext.JobId;

        public TimeSpan ElapsedTime => _jobContext.ElapsedTime;

        public JobStatus Status
        {
            get
            {
                switch (_task.Status)
                {
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForChildrenToComplete:
                        return JobStatus.Running;

                    case TaskStatus.Faulted:
                        return JobStatus.Faulted;

                    case TaskStatus.RanToCompletion:
                        return JobStatus.RanToCompletion;

                    case TaskStatus.Canceled:
                        return JobStatus.Canceled;

                    case TaskStatus.Created:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingToRun:
                        return JobStatus.Created;

                    default:
                        return JobStatus.Created;
                }
            }
        }

        async Task JobHandle.Cancel()
        {
            _jobContext.Cancel();

            try
            {
                await _task.OrTimeout(TimeSpan.FromSeconds(30)).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
            }
            catch (OperationCanceledException)
            {
            }
        }

        public Task NotifyCanceled(string reason = null)
        {
            return _jobContext.NotifyCanceled(reason);
        }

        public Task NotifyStarted(Uri managementAddress)
        {
            return _jobContext.NotifyStarted(managementAddress);
        }

        public Task NotifyCompleted()
        {
            return _jobContext.NotifyCompleted();
        }

        public Task NotifyFaulted(Exception exception)
        {
            return _jobContext.NotifyFaulted(exception);
        }

        public T Command => _jobContext.Command;
    }
}
