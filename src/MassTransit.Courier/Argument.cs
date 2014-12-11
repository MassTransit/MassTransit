// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier
{
    using System;
    using System.Linq.Expressions;
    using System.Security.Cryptography;


    public interface Argument
    {
        /// <summary>
        /// True if the argument has a value, false if the value is null in the routing slip
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// True if the argument was present in the intinerary
        /// </summary>
        bool IsPresent { get; }
    }


    /// <summary>
    /// An argument that may have a domain-specific implementation that goes beyond simple
    /// type usage. For instance, encrypted values maybe mapped
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Argument<out T> :
        Argument
    {
        /// <summary>
        /// The argument value
        /// </summary>
        T Value { get; }
    }


    public interface ArgumentConfigurator<T, TArguments>
        where TArguments : class
    {
        void Encrypted(byte[] key, byte[] iv);
    }


    class ArgumentConfiguratorImpl<T, TArguments> : 
        ArgumentConfigurator<T, TArguments>
        where TArguments : class
    {
        public void Encrypted(byte[] key, byte[] iv)
        {
            using (var x = new AesCryptoServiceProvider())
            {
                x.Key = key;
                x.IV = iv;
            }
        }
    }


    public abstract class ArgumentMap<TArguments>
        where TArguments : class
    {
        public virtual void Map<T>(Expression<Func<TArguments, T>> propertyExpression, Action<ArgumentConfigurator<T, TArguments>> configure)
        {
            var configurator = new ArgumentConfiguratorImpl<T, TArguments>();

            configure(configurator);
        }
    }
}