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
namespace MassTransit.Turnout.Contracts
{
    using System;


    /// <summary>
    /// Published when a job faults
    /// </summary>
    public interface JobFaulted
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The time the job faulted
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The exceptions that were thrown by the job
        /// </summary>
        ExceptionInfo Exceptions { get; }
    }

    /// <summary>
    /// Published when a job faults
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface JobFaulted<out TInput> :
        JobFaulted
    {
        /// <summary>
        /// The job input
        /// </summary>
        TInput Input { get; }
    }
}