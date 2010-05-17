// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Distributor
{
    using System.Collections.Generic;

    public interface IWorkerSelectionStrategy<T>
        where T : class
    {
        /// <summary>
        /// Indicates if a given message can be assigned to a worker in distribution
        /// </summary>
        /// <param name="workerCandidates">List of possible candidates</param>
        /// <param name="message">Message in question</param>
        /// <returns>True if there is a worker avaiable to send the message to</returns>
        bool CanBeAssignedToWorker(IEnumerable<WorkerDetails> workerCandidates, T message);

        /// <summary>
        /// Finds a worker avaiable to send a given message to
        /// </summary>
        /// <param name="workerCandidates">List of possible candidates</param>
        /// <param name="message">Message in question</param>
        /// <returns>A worker, if one is avaiable. If there are no acceptable workers in the 
        /// candidate list, <c>null</c> is returned.</returns>
        WorkerDetails AssignToWorker(IEnumerable<WorkerDetails> workerCandidates, T message);
    }
}