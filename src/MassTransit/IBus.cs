// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Pipeline;

    /// <summary>
    /// A bus is a logical element that includes a local endpoint and zero or more receive endpoints
    /// </summary>
    public interface IBus :
        IPublishEndpoint
    {
        /// <summary>
        /// The receive address of the bus itself, versus any receive endpoints that were created
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// The inbound pipe for the bus
        /// </summary>
        IConsumePipe ConsumePipe { get; }

        /// <summary>
        /// Retrieve a destination endpoint
        /// </summary>
        /// <param name="address">The endpoint address</param>
        /// <returns>A sendable endpoint</returns>
        Task<ISendEndpoint> GetSendEndpoint(Uri address);
    }
}