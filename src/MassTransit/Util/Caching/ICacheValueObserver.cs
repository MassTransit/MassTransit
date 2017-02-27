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
    /// <summary>
    /// Observes behavior within the cache
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface ICacheValueObserver<TValue>
        where TValue : class
    {
        /// <summary>
        /// Called when a new node is added to the cache, after the node has resolved.
        /// </summary>
        /// <param name="node">The cached node</param>
        /// <param name="value">The cached value, to avoid awaiting</param>
        /// <returns>An awaitable task for the observer</returns>
        void ValueAdded(INode<TValue> node, TValue value);

        /// <summary>
        /// Called when a node is removed from the cache.
        /// </summary>
        /// <param name="node">The cached node</param>
        /// <param name="value">The cached value, to avoid awaiting</param>
        /// <returns>An awaitable task for the observer</returns>
        void ValueRemoved(INode<TValue> node, TValue value);

        /// <summary>
        /// Called when the cache is cleared of all nodes.
        /// </summary>
        void CacheCleared();
    }
}