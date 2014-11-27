// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SubscriptionConnectors
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Configuration;
    using Pipeline;
    using Policies;


    public class ConsumerConnectorCache<T> :
        IConsumerConnectorCache
        where T : class
    {
        readonly Lazy<ConsumerConnector<T>> _connector;


        ConsumerConnectorCache()
        {
            _connector = new Lazy<ConsumerConnector<T>>(() => new ConsumerConnector<T>(),
                LazyThreadSafetyMode.PublicationOnly);
        }

        public static ConsumerConnector Connector
        {
            get { return InstanceCache.Cached.Value.Connector; }
        }

        ConsumerConnector IConsumerConnectorCache.Connector
        {
            get { return _connector.Value; }
        }


        static class InstanceCache
        {
            internal static readonly Lazy<IConsumerConnectorCache> Cached = new Lazy<IConsumerConnectorCache>(
                () => new ConsumerConnectorCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }


    public static class ConsumerConnectorCache
    {
        public static ConsumerConnector GetConsumerConnector<T>()
            where T : class
        {
            return InstanceCache.Cached.GetOrAdd(typeof(T), x => new CachedConnector<T>()).Connector;
        }

        public static ConsumerConnector GetConsumerConnector(Type type)
        {
            return GetOrAdd(type).Connector;
        }

        static CachedConnector GetOrAdd(Type type)
        {
            return InstanceCache.Cached.GetOrAdd(type, _ =>
                (CachedConnector)Activator.CreateInstance(typeof(CachedConnector<>).MakeGenericType(type)));
        }

        public static ConnectHandle Connect(IConsumePipe consumePipe, Type consumerType, Func<Type, object> objectFactory,
            IRetryPolicy retryPolicy)
        {
            return GetOrAdd(consumerType).Connect(consumePipe, objectFactory, retryPolicy);
        }


        interface CachedConnector
        {
            ConsumerConnector Connector { get; }

            ConnectHandle Connect(IConsumePipe consumePipe, Func<Type, object> objectFactory, IRetryPolicy retryPolicy);
        }


        class CachedConnector<T> :
            CachedConnector
            where T : class
        {
            readonly Lazy<ConsumerConnector> _connector;

            public CachedConnector()
            {
                _connector = new Lazy<ConsumerConnector>(() => ConsumerConnectorCache<T>.Connector);
            }

            public ConsumerConnector Connector
            {
                get { return _connector.Value; }
            }

            public ConnectHandle Connect(IConsumePipe consumePipe, Func<Type, object> objectFactory, IRetryPolicy retryPolicy)
            {
                var consumerFactory = new ObjectConsumerFactory<T>(objectFactory);

                return _connector.Value.Connect(consumePipe, consumerFactory, retryPolicy);
            }
        }


        static class InstanceCache
        {
            internal static readonly ConcurrentDictionary<Type, CachedConnector> Cached = new ConcurrentDictionary<Type, CachedConnector>();
        }
    }
}