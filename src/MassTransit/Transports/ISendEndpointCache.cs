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
namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    public interface ISendEndpointCache<TKey>
    {
        /// <summary>
        /// Return a SendEndpoint from the cache, using the factory to create it if it doesn't exist in the cache.
        /// </summary>
        /// <param name="key">The key for the endpoint</param>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<ISendEndpoint> GetSendEndpoint(TKey key, SendEndpointFactory<TKey> factory);
    }
}