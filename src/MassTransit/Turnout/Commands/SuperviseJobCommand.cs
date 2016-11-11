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
namespace MassTransit.Turnout.Commands
{
    using System;
    using Contracts;


    class SuperviseJobCommand<T> :
        SuperviseJob<T>
        where T : class
    {
        public SuperviseJobCommand(Guid jobId, Guid executionId, T job, DateTime lastStatusTimestamp, JobStatus lastStatus)
        {
            JobId = jobId;
            LastStatusTimestamp = lastStatusTimestamp;
            LastStatus = lastStatus;
            ExecutionId = executionId;
            Job = job;
        }

        public Guid JobId { get; private set; }

        public DateTime LastStatusTimestamp { get; private set; }

        public JobStatus LastStatus { get; private set; }

        public Guid ExecutionId { get; private set; }

        public T Job { get; private set; }
    }
}