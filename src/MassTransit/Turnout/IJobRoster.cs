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

    /// <summary>
    /// Contains the roster of active jobs for the receive endpoint
    /// </summary>
    public interface IJobRoster
    {
        bool TryGetJob(Guid jobId, out JobHandle jobReference);

        /// <summary>
        /// Add a job to the registry
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobReference"></param>
        void Add(Guid jobId, JobHandle jobReference);

        /// <summary>
        /// Remove the job from the roster
        /// </summary>
        /// <param name="jobId"></param>
        void RemoveJob(Guid jobId);
    }
}