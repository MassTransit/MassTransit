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
    /// The missing value factory is used by default if not specified inline, in the case
    /// a value is not found in the index this will be used to create it.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface IMissingValueFactory<TValue>
        where TValue : class
    {
        /// <summary>
        /// Create the missing value
        /// </summary>
        /// <param name="key">The missing key</param>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <returns>The created value, once it becomes available</returns>
        Task<TValue> CreateMissingValue<TKey>(TKey key);
    }
}