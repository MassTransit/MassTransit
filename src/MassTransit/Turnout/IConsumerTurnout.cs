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
    using System.Threading.Tasks;


    /// <summary>
    /// A turnout is a lane where buses can pull aside to wait for passengers. In that style, the
    /// turnout service allows message consumers to run asynchronous tasks and continue once the background job
    /// is completed.
    /// </summary>
    public interface IConsumerTurnout
    {
        /// <summary>
        /// Creates a job and schedules it for execution.
        /// </summary>
        /// <typeparam name="T">The message type that is used to initiate the job</typeparam>
        /// <param name="context">The context of the message being consumed</param>
        /// <param name="jobFactory">The factory to create the job Task</param>
        /// <returns>A tram job</returns>
        Task<TramJob<T>> CreateJob<T>(ConsumeContext<T> context, IJobFactory<T> jobFactory)
            where T : class;
    }
}