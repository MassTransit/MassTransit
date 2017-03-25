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
namespace MassTransit.ConsumeConnectors
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public static class ConsumerConventionCache
    {
        static ConsumerConventionCache()
        {
            ConsumerConvention.Register<AsyncConsumerConvention>();
            ConsumerConvention.Register<LegacyConsumerConvention>();
        }

        public static void Add(string conventionName, IConsumerConvention convention)
        {
            Cached.Instance.AddOrUpdate(conventionName, add => convention, (_, update) => convention);
        }

        public static void Remove(string conventionName)
        {
            IConsumerConvention _;
            Cached.Instance.TryRemove(conventionName, out _);
        }

        /// <summary>
        /// Returns the conventions registered for identifying message consumer types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<IConsumerMessageConvention> GetConventions<T>()
            where T : class
        {
            foreach (var convention in Cached.Instance.Values)
            {
                yield return convention.GetConsumerMessageConvention<T>();
            }
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<string, IConsumerConvention> Instance = new ConcurrentDictionary<string, IConsumerConvention>();
        }
    }
}