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
namespace MassTransit.ConsumeConnectors
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Pipeline;
    using Policies;


    public class ConsumerConnectorCache<TConsumer> :
        IConsumerConnectorCache
        where TConsumer : class
    {
        readonly Lazy<ConsumerConnector<TConsumer>> _connector;

        ConsumerConnectorCache()
        {
            _connector = new Lazy<ConsumerConnector<TConsumer>>(() => new ConsumerConnector<TConsumer>(),
                LazyThreadSafetyMode.PublicationOnly);
        }

        public static ConsumerConnector Connector
        {
            get { return Cached.Instance.Value.Connector; }
        }

        ConsumerConnector IConsumerConnectorCache.Connector
        {
            get { return _connector.Value; }
        }


        static class Cached
        {
            internal static readonly Lazy<IConsumerConnectorCache> Instance = new Lazy<IConsumerConnectorCache>(
                () => new ConsumerConnectorCache<TConsumer>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }


    public static class ConsumerConnectorCache
    {
        public static ConsumerConnector GetConsumerConnector<T>()
            where T : class
        {
            return Cached.Instance.GetOrAdd(typeof(T), x => new CachedConnector<T>()).Connector;
        }

        static CachedConnector GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedConnector)Activator.CreateInstance(typeof(CachedConnector<>).MakeGenericType(type)));
        }

        public static ConnectHandle Connect(IConsumePipeConnector consumePipe, Type consumerType, Func<Type, object> objectFactory,
            IRetryPolicy retryPolicy)
        {
            return GetOrAdd(consumerType).Connect(consumePipe, objectFactory, retryPolicy);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConnector> Instance =
                new ConcurrentDictionary<Type, CachedConnector>();
        }


        interface CachedConnector
        {
            ConsumerConnector Connector { get; }

            ConnectHandle Connect(IConsumePipeConnector consumePipe, Func<Type, object> objectFactory, IRetryPolicy retryPolicy);
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

            public ConnectHandle Connect(IConsumePipeConnector consumePipe, Func<Type, object> objectFactory, IRetryPolicy retryPolicy)
            {
                var consumerFactory = new ObjectConsumerFactory<T>(objectFactory);

                return _connector.Value.Connect(consumePipe, consumerFactory, retryPolicy);
            }
        }
    }
}