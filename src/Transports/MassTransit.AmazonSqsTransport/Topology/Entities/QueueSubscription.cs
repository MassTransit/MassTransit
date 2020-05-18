// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport.Topology.Entities
{
    /// <summary>
    /// The topic to queue binding details to declare the binding to AmazonSQS
    /// </summary>
    public interface QueueSubscription
    {
        /// <summary>
        /// The topic
        /// </summary>
        Topic Source { get; }

        /// <summary>
        /// The queue
        /// </summary>
        Queue Destination { get; }
    }
}