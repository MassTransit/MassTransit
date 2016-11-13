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
    using System.Collections.Generic;


    /// <summary>
    /// Published when a job is started
    /// </summary>
    public interface JobStarted
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The time the job was started
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Zero if the job is being started for the first time, otherwise, the number of previous failures
        /// </summary>
        int RetryCount { get; }

        /// <summary>
        /// The management address on which this job was started
        /// </summary>
        Uri ManagementAddress { get; }

        /// <summary>
        /// The arguments used to create the job (The job message type serialized)
        /// </summary>
        IDictionary<string, object> Arguments { get; }
    }
}