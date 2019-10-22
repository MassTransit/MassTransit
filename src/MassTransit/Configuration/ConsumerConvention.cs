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
    using System;
    using ConsumeConnectors;
    using Metadata;
    using Util;


    /// <summary>
    /// Used to register conventions for consumer message types
    /// </summary>
    public static class ConsumerConvention
    {
        /// <summary>
        /// Register a consumer convention to be used for finding message types
        /// </summary>
        /// <typeparam name="T">The convention type</typeparam>
        public static void Register<T>()
            where T : IConsumerConvention, new()
        {
            var convention = new T();

            ConsumerConventionCache.Add(TypeMetadataCache<T>.ShortName, convention);
        }

        /// <summary>
        /// Register a consumer convention to be used for finding message types
        /// </summary>
        /// <typeparam name="T">The convention type</typeparam>
        public static void Register<T>(T convention)
            where T : IConsumerConvention
        {
            if (convention == null)
                throw new ArgumentNullException(nameof(convention));

            ConsumerConventionCache.Add(TypeMetadataCache<T>.ShortName, convention);
        }

        /// <summary>
        /// Remove a consumer convention used for finding message types
        /// </summary>
        /// <typeparam name="T">The convention type to remove</typeparam>
        public static void Remove<T>()
            where T : IConsumerConvention
        {
            ConsumerConventionCache.Remove(TypeMetadataCache<T>.ShortName);
        }
    }
}