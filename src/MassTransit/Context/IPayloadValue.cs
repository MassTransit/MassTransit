// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;


    /// <summary>
    /// A property is a value stored in the context, which can be accessed by name or
    /// by type. This is the actual property storage element
    /// </summary>
    public interface IPayloadValue
    {
        /// <summary>
        /// The property value type
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Returns the value if it can be assigned to the specified type
        /// </summary>
        /// <typeparam name="T">The requested type</typeparam>
        /// <param name="value">The output value</param>
        /// <returns></returns>
        bool TryGetValue<T>(out T value)
            where T : class;
    }


    /// <summary>
    /// A property value with the generic type applied
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPayloadValue<out T> :
        IPayloadValue
        where T : class
    {
        /// <summary>
        /// The value of the property, already assigned to T
        /// </summary>
        T Value { get; }
    }
}