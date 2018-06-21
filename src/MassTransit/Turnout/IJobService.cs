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


    /// <summary>
    /// A turnout is a lane where buses can pull aside to wait for passengers. In that style, the
    /// turnout service allows message consumers to run asynchronous tasks and continue once the background job
    /// is completed.
    /// </summary>
    public interface IJobService
    {
        /// <summary>
        /// The input address of the job scheduler
        /// </summary>
        Uri InputAddress { get; }

        /// <summary>
        /// Creates a job and schedules it for execution.
        /// </summary>
        /// <typeparam name="T">The message type that is used to initiate the job</typeparam>
        /// <param name="context">The context of the message being consumed</param>
        /// <param name="command">The job command</param>
        /// <param name="jobFactory">The factory to create the job Task</param>
        /// <param name="jobId">Identifier for the job</param>
        /// <returns>The newly created job's handle</returns>
        Task<JobHandle<T>> CreateJob<T>(ConsumeContext context, Guid jobId, T command, IJobFactory<T> jobFactory)
            where T : class;

        /// <summary>
        /// Schedules a supervision message for the job, on the job service endpoint, which
        /// will expire if not serviced within the expected timeframe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="job"></param>
        /// <param name="jobHandle"></param>
        /// <returns></returns>
        Task ScheduleSupervision<T>(ConsumeContext context, T job, JobHandle jobHandle)
            where T : class;

        /// <summary>
        /// Shut town the job service, cancelling any pending jobs
        /// </summary>
        Task Stop();
    }
}