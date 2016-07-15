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
    /// Send to the monitor queue so that any running node can monitor the queue of a worker node
    /// and handle the job if the original node fails to read the queue.
    /// </summary>
    public interface MonitorJob
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The address of the job host
        /// </summary>
        Uri JobHostAddress { get; }

        /// <summary>
        /// The message identifier of the matching SuperviseJob message
        /// </summary>
        Guid MessageId { get; }
    }
}