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
namespace MassTransit.Topology
{
    using System;


    /// <summary>
    /// A receive endpoint has a topology that is used within the context of a received message
    /// </summary>
    public interface IReceiveEndpointTopology
    {
        /// <summary>
        /// The input address of the receive endpoint
        /// </summary>
        Uri InputAddress { get; }

        ISendTopology Send { get; }

        IPublishTopology Publish { get; }

        /// <summary>
        /// Creates a send endpoint provider
        /// </summary>
        /// <value></value>
        ISendEndpointProvider SendEndpointProvider { get; }

        /// <summary>
        /// Creates a publish endpoint provider
        /// </summary>
        /// <value></value>
        IPublishEndpointProvider PublishEndpointProvider { get; }

        /// <summary>
        /// Returns the send transport provider
        /// </summary>
        ISendTransportProvider SendTransportProvider { get; }
    }
}