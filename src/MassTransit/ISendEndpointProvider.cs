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
namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// The Send Endpoint Provider is used to retrieve endpoints using addresses. The interface is
    /// available both at the bus and within the context of most message receive handlers, including
    /// the consume context, saga context, consumer context, etc. The most local provider should be
    /// used to ensure message continuity is maintained.
    /// </summary>
    public interface ISendEndpointProvider :
        ISendObserverConnector
    {
        /// <summary>
        /// Return the send endpoint for the specified address
        /// </summary>
        /// <param name="address">The endpoint address</param>
        /// <returns>The send endpoint</returns>
        Task<ISendEndpoint> GetSendEndpoint(Uri address);
    }
}