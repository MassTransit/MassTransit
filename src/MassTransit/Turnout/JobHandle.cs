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


    public interface JobHandle<out T> :
        JobHandle
        where T : class
    {
        /// <summary>
        /// The command that initiated the job
        /// </summary>
        T Command { get; }
    }


    /// <summary>
    /// A JobHandle contains the JobContext, Task, and provides access to the job control
    /// </summary>
    public interface JobHandle
    {
        /// <summary>
        /// The unique job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The time the job has been active
        /// </summary>
        TimeSpan ElapsedTime { get; }

        /// <summary>
        /// The job's status, derived from the underlying Task status
        /// </summary>
        JobStatus Status { get; }

        /// <summary>
        /// Cancel the job task
        /// </summary>
        /// <returns></returns>
        Task Cancel();

        Task NotifyCanceled(string reason);

        Task NotifyStarted(Uri managementAddress);

        Task NotifyCompleted();

        Task NotifyFaulted(Exception exception);
    }
}