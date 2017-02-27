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
    /// A pending Get on an index, which has yet to be processed. Used by the
    /// node value factory to sequentially resolve the value for an index item
    /// which is then added to the cache.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface IPendingValue<TValue>
        where TValue : class
    {
        /// <summary>
        /// Sets the pending value, eliminating the need for the factory method.
        /// </summary>
        /// <param name="value">The resolved value</param>
        void SetValue(Task<TValue> value);

        /// <summary>
        /// Create the value using the missing value factory supplied to Get
        /// </summary>
        /// <returns>Either the value, or a faulted task.</returns>
        Task<TValue> CreateValue();
    }
}