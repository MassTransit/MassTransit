// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;


    /// <summary>
    /// Headers are used to store value outside of a message body that are transported with 
    /// the message content.
    /// </summary>
    public interface Headers
    {
        /// <summary>
        /// Returns all available headers
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> GetAll();

        /// <summary>
        /// If the specified header name is found, returns the value of the header as an object
        /// </summary>
        /// <param name="key">The header name</param>
        /// <param name="value">The output header value</param>
        /// <returns>True if the header was found, otherwise false</returns>
        bool TryGetHeader(string key, out object value);

        /// <summary>
        /// Returns the specified header as the type, returning a default value is the header is not found
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="key">The header name</param>
        /// <param name="defaultValue">The default value of the header if not found</param>
        /// <returns>The header value</returns>
        T Get<T>(string key, T defaultValue = default)
            where T : class;

        /// <summary>
        /// Returns the specified header as the type, returning a default value is the header is not found
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="key">The header name</param>
        /// <param name="defaultValue">The default value of the header if not found</param>
        /// <returns>The header value</returns>
        T? Get<T>(string key, T? defaultValue = default)
            where T : struct;
    }
}