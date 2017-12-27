// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Topology.Builders
{
    using Entities;


    /// <summary>
    /// A unique builder context should be created for each specification, so that the items added
    /// by it can be combined together into a group - so that if a subsequent specification yanks 
    /// something that conflicts, the system can yank the group or warn that it's impacted.
    /// </summary>
    public interface ISubscriptionEndpointBrokerTopologyBuilder :
        IBrokerTopologyBuilder
    {
        /// <summary>
        /// A handle to the subscription topic
        /// </summary>
        TopicHandle Topic { get; }
    }
}