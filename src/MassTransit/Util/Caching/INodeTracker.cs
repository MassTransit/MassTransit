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
    using System.Collections.Generic;


    public interface INodeTracker<TValue> :
        IConnectCacheValueObserver<TValue>
        where TValue : class
    {
        /// <summary>
        /// Maintains statistics for the cache
        /// </summary>
        CacheStatistics Statistics { get; }

        /// <summary>
        /// Adds a pending node to the cache, that once resolved, is published
        /// to the indices
        /// </summary>
        /// <param name="nodeValueFactory"></param>
        void Add(INodeValueFactory<TValue> nodeValueFactory);

        /// <summary>
        /// Just add the value, straight up
        /// </summary>
        /// <param name="value"></param>
        void Add(TValue value);

        /// <summary>
        /// Assigns the node to the current bucket, likely do it being touched.
        /// </summary>
        /// <param name="node"></param>
        void Rebucket(IBucketNode<TValue> node);

        /// <summary>
        /// Remove a node from the cache, notifying all observers that it was removed
        /// (which updates the indices as well).
        /// </summary>
        /// <param name="existingNode">The node being removed</param>
        void Remove(INode<TValue> existingNode);

        /// <summary>
        /// Returns every known node in the cache from the valid buckets
        /// </summary>
        /// <returns></returns>
        IEnumerable<INode<TValue>> GetAll();

        /// <summary>
        /// Clear the cache, throw out the buckets, time to start over
        /// </summary>
        void Clear();
    }
}