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
namespace MassTransit.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Contracts;


    public class ConsumerJobHandle :
        JobHandle
    {
        readonly JobContext _jobContext;
        readonly Task _task;
        TimeSpan _elapsedTime;

        public ConsumerJobHandle(JobContext jobContext, Task task)
        {
            _jobContext = jobContext;
            _task = task;
        }

        public Guid JobId => _jobContext.JobId;

        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public JobStatus Status
        {
            get
            {
                switch (_task.Status)
                {
                    case TaskStatus.Running:
                        return JobStatus.Running;

                    case TaskStatus.Faulted:
                        return JobStatus.Faulted;

                    case TaskStatus.RanToCompletion:
                        return JobStatus.RanToCompletion;

                    case TaskStatus.Canceled:
                        return JobStatus.Canceled;

                    default:
                        return JobStatus.Created;
                }
            }
        }

        public Task Cancel()
        {
            throw new NotImplementedException();
        }
    }
}