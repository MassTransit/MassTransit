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
namespace MassTransit.Internal
{
    using System;
    using Subscriptions;

    public interface IDispatcherContext
    {
        IObjectBuilder Builder { get; }
        IServiceBus Bus { get; }
        ISubscriptionCache SubscriptionCache { get; }

        ISubscriptionTypeInfo GetSubscriptionTypeInfo(Type type);

        void Attach<TMessage>(Consumes<TMessage>.All consumer) where TMessage : class;
        void Detach<TMessage>(Consumes<TMessage>.All consumer) where TMessage : class;

        void Consume(object message);

        T GetDispatcher<T>() where T : class;
        T GetDispatcher<T>(Type type) where T : class;

        void AddSubscription(Subscription subscription);
        void RemoveSubscription(Subscription subscription);
    }
}