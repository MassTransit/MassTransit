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
    using System.Threading.Tasks;


    public interface IPublishEndpointProvider :
        IPublishObserverConnector
    {
        /// <summary>
        /// Creates a publish endpoint, using the optional <see cref="ConsumeContext"/> for the correlation identifiers
        /// </summary>
        /// <param name="sourceAddress"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, ConsumeContext context = null);

        /// <summary>
        /// Return the SendEndpoint used for publishing the specified message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<ISendEndpoint> GetPublishSendEndpoint<T>(T message)
            where T : class;
    }
}