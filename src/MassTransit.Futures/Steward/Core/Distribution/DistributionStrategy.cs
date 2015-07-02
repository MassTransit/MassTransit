// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Steward.Core.Distribution
{
    using System.Collections.Generic;


    /// <summary>
    /// A distribution strategy is used to distribute messages across a set of nodes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface DistributionStrategy<T>
    {
        /// <summary>
        /// Returns the node for the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T this[string key] { get; }

        /// <summary>
        /// Adds a range of nodes to the distribution pool
        /// </summary>
        /// <param name="nodes">The nodes to add</param>
        void Init(IEnumerable<T> nodes);

        /// <summary>
        /// Adds a node to the distribution pool
        /// </summary>
        /// <param name="node">The node to add</param>
        void Add(T node);

        /// <summary>
        /// Remove a node (and all of its replicas) from the distribution pool
        /// </summary>
        /// <param name="node">The node to remove</param>
        void Remove(T node);

        /// <summary>
        /// Returns the node for the given data block
        /// </summary>
        /// <param name="data">The data block used to select the node</param>
        /// <returns>The element for the specified data block</returns>
        T GetNode(byte[] data);
    }
}