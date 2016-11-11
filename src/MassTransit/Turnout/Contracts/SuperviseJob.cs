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
namespace MassTransit.Turnout.Contracts
{
    using System;


    /// <summary>
    /// Supervise an active job
    /// Sent to the node where the job is executing so that it can check the status
    /// of the job. Sent at an interval until the job is completed.
    /// </summary>
    public interface SuperviseJob<out T>
        where T : class
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The time of the last job status check
        /// </summary>
        DateTime LastUpdated { get; }

        /// <summary>
        /// The previous job status
        /// </summary>
        JobStatus LastStatus { get; }

        /// <summary>
        /// The job command, which initiated the job
        /// </summary>
        T Command { get; }
    }
}