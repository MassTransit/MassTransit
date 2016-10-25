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
namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;


    public interface IReceiveEndpointCollection :
        IReceiveObserverConnector,
        IReceiveEndpointObserverConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IEnumerable<IReceiveEndpoint>,
        IProbeSite
    {
        /// <summary>
        /// Add an endpoint to the collection
        /// </summary>
        /// <param name="endpointName"></param>
        /// <param name="endpoint"></param>
        void Add(string endpointName, IReceiveEndpoint endpoint);

        /// <summary>
        /// Start all endpoints in the collection which have not been started, and return the handles
        /// for those endpoints.
        /// </summary>
        /// <returns></returns>
        Task<BusReceiveEndpointHandle[]> StartEndpoints();

        /// <summary>
        /// Start a new receive endpoint
        /// </summary>
        /// <param name="endpointName"></param>
        /// <returns></returns>
        Task<BusReceiveEndpointHandle> Start(string endpointName);
    }
}