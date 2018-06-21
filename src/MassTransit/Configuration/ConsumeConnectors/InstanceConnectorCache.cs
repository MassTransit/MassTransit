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
    using System;
    using System.Collections.Concurrent;
    using System.Threading;


    public class InstanceConnectorCache<T> :
        IInstanceConnectorCache<T>
        where T : class
    {
        readonly Lazy<InstanceConnector<T>> _connector;

        InstanceConnectorCache()
        {
            _connector = new Lazy<InstanceConnector<T>>(() => new InstanceConnector<T>(),
                LazyThreadSafetyMode.PublicationOnly);
        }

        public static IInstanceConnector Connector => InstanceCache.Cached.Value.Connector;

        IInstanceConnector IInstanceConnectorCache<T>.Connector => _connector.Value;


        static class InstanceCache
        {
            internal static readonly Lazy<IInstanceConnectorCache<T>> Cached = new Lazy<IInstanceConnectorCache<T>>(
                () => new InstanceConnectorCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }


    public static class InstanceConnectorCache
    {
        public static IInstanceConnector GetInstanceConnector<T>()
            where T : class
        {
            return InstanceCache.Cached.Value.GetOrAdd(typeof(T),
                _ => new Lazy<IInstanceConnector>(() => InstanceConnectorCache<T>.Connector)).Value;
        }

        public static IInstanceConnector GetInstanceConnector(Type type)
        {
            return InstanceCache.Cached.Value.GetOrAdd(type, _ => new Lazy<IInstanceConnector>(() =>
                (IInstanceConnector)Activator.CreateInstance(typeof(InstanceConnector<>).MakeGenericType(type)))).Value;
        }


        static class InstanceCache
        {
            internal static readonly Lazy<ConcurrentDictionary<Type, Lazy<IInstanceConnector>>> Cached =
                new Lazy<ConcurrentDictionary<Type, Lazy<IInstanceConnector>>>(
                    () => new ConcurrentDictionary<Type, Lazy<IInstanceConnector>>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}