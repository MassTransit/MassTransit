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
namespace MassTransit.Util.Caching
{
    using System.Threading.Tasks;


    /// <summary>
    /// An index is used to access items in the cache quickly
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IIndex<TKey, TValue>
        where TValue : class
    {
        Task<TValue> Get(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory = null);

        /// <summary>
        /// Forcibly removes the item from the cache, but disposal may occur asynchronously.
        /// </summary>
        /// <param name="key">The value key</param>
        bool Remove(TKey key);
    }
}