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
    using GreenPipes;


    public interface JobContext :
        PipeContext,
        MessageContext,
        ISendEndpointProvider,
        IPublishEndpoint
    {
        /// <summary>
        /// Identifies the job execution
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The elapsed time of the job
        /// </summary>
        TimeSpan ElapsedTime { get; }

        Task NotifyCanceled(string reason = null);

        Task NotifyStarted(Uri managementAddress);

        Task NotifyCompleted();

        Task NotifyFaulted(Exception exception);
    }


    public interface JobContext<out TCommand> :
        JobContext
        where TCommand : class
    {
        /// <summary>
        /// The message that initiated the job
        /// </summary>
        TCommand Command { get; }
    }


    public interface JobContext<out TCommand, TResult> :
        JobContext<TCommand>
        where TCommand : class
        where TResult : class
    {
        /// <summary>
        /// Create a result context from the job context to pass to the next pipe
        /// </summary>
        /// <param name="result">The job's result</param>
        /// <returns></returns>
        JobResultContext<TCommand, TResult> FromResult(TResult result);
    }
}