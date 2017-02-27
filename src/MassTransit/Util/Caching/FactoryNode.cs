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
    /// A factory node is a temporary node used by an index until the node has
    /// been resolved.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public class FactoryNode<TValue> :
        INode<TValue>
        where TValue : class
    {
        readonly INodeValueFactory<TValue> _nodeValueFactory;

        public FactoryNode(INodeValueFactory<TValue> nodeValueFactory)
        {
            _nodeValueFactory = nodeValueFactory;
        }

        public Task<TValue> Value => _nodeValueFactory.Value;

        public bool HasValue => _nodeValueFactory.Value.IsCompleted;

        public Task<TValue> GetValue(IPendingValue<TValue> pendingValue)
        {
            _nodeValueFactory.Add(pendingValue);

            return _nodeValueFactory.Value;
        }
    }
}