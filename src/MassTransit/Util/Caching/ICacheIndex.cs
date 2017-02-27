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


    public interface ICacheIndex<TValue>
        where TValue : class
    {
        /// <summary>
        /// Clear the index, removing all nodes, but leaving them unmodified
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds a node to the index
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>True if the value was added, false if the value already existed in the index</returns>
        Task<bool> Add(INode<TValue> node);

        /// <summary>
        /// Check if the value is in the index, and if found, return the node
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="node">The matching node</param>
        /// <returns>True if the value was found, otherwise false</returns>
        bool TryGetExistingNode(TValue value, out INode<TValue> node);
    }
}