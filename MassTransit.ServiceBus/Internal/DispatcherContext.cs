// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.Internal
{
    using System;
    using Subscriptions;

    public class DispatcherContext :
        IDispatcherContext
    {
        private readonly IObjectBuilder _builder;
        private readonly IServiceBus _bus;
        private readonly ISubscriptionCache _cache;
        private readonly TypeInfoCache _typeInfoCache;
        private readonly IMessageTypeDispatcher _dispatcher;

        public DispatcherContext(IObjectBuilder builder, IServiceBus bus, IMessageTypeDispatcher dispatcher, ISubscriptionCache cache, TypeInfoCache typeInfoCache)
        {
            _builder = builder;
            _typeInfoCache = typeInfoCache;
            _cache = cache;
            _dispatcher = dispatcher;
            _bus = bus;
        }

        public IObjectBuilder Builder
        {
            get { return _builder; }
        }

        public IServiceBus Bus
        {
            get { return _bus; }
        }

        public ISubscriptionCache SubscriptionCache
        {
            get { return _cache; }
        }

        public ISubscriptionTypeInfo GetSubscriptionTypeInfo(Type type)
        {
            return _typeInfoCache.GetSubscriptionTypeInfo(type);
        }

        public void Attach<TMessage>(Consumes<TMessage>.All consumer) where TMessage : class
        {
            _dispatcher.Attach(consumer);
        }

        public void Detach<TMessage>(Consumes<TMessage>.All consumer) where TMessage : class
        {
            _dispatcher.Detach(consumer);
        }

        public void Consume(object message)
        {
            _dispatcher.Consume(message);
        }

        public T GetDispatcher<T>() where T : class
        {
            return _dispatcher.GetDispatcher<T>();
        }

        public T GetDispatcher<T>(Type type) where T : class
        {
            return _dispatcher.GetDispatcher<T>(type);
        }

        public void AddSubscription(Subscription subscription)
        {
            _cache.Add(subscription);
        }

        public void RemoveSubscription(Subscription subscription)
        {
            _cache.Remove(subscription);
        }
    }
}