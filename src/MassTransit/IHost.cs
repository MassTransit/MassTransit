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
namespace MassTransit
{
    using System;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Transports;


    /// <summary>
    /// A Bus Host is a transport-neutral reference to a host
    /// </summary>
    public interface IHost :
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IReceiveObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IReceiveEndpointObserverConnector,
        IProbeSite
    {
        /// <summary>
        /// An address that identifies the host
        /// </summary>
        Uri Address { get; }
        
        IHostTopology Topology { get; }
    }
}