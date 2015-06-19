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
namespace MassTransit.Monitoring.Introspection
{
    using System.Collections.Generic;
    using System.Threading;


    /// <summary>
    /// Passed to a probe site to inspect it for interesting things
    /// </summary>
    public interface ProbeContext
    {
        /// <summary>
        /// If for some reason the probe is cancelled, allowing an early withdrawl
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Add a key/value pair to the current probe context
        /// </summary>
        /// <param name="key">The key name</param>
        /// <param name="value">The value</param>
        void Add(string key, string value);

        /// <summary>
        /// Add a key/value pair to the current probe context
        /// </summary>
        /// <param name="key">The key name</param>
        /// <param name="value">The value</param>
        void Add(string key, object value);
        
        /// <summary>
        /// Add the properties of the object as key/value pairs to the current context
        /// </summary>
        /// <param name="values">The object (typically anonymous with new{}</param>
        void Set(object values);

        /// <summary>
        /// Add the values from the enumeration as key/value pairs
        /// </summary>
        /// <param name="values"></param>
        void Set(IEnumerable<KeyValuePair<string, object>> values);

        ProbeContext CreateScope(string key);
    }
}