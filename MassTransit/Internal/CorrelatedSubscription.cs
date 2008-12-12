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
    using Exceptions;
    using Subscriptions;

    public class CorrelatedSubscription<TComponent, TMessage, TKey> :
        ISubscriptionTypeInfo
        where TComponent : class, Consumes<TMessage>.All
        where TMessage : class, CorrelatedBy<TKey>
    {
        public void Subscribe<T>(IDispatcherContext context, T component) where T : class
        {
            Consumes<TMessage>.For<TKey> consumer = GetTypedConsumer(component);

            _Subscribe(context, consumer);
        }

        public void Unsubscribe<T>(IDispatcherContext context, T component) where T : class
        {
            Consumes<TMessage>.For<TKey> consumer = GetTypedConsumer(component);

            _Unsubscribe(context, consumer);
        }

        public void AddComponent(IDispatcherContext context)
        {
            GetComponentDispatcher(context);
        }

        public void RemoveComponent(IDispatcherContext context)
        {
            throw new NotSupportedException("We can't remove right now");
        }

        public void Dispose()
        {
        }

        private static Consumes<TMessage>.For<TKey> GetTypedConsumer<T>(T component)
        {
            Consumes<TMessage>.For<TKey> consumer = component as Consumes<TMessage>.For<TKey>;
            if (consumer == null)
                throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof (T), typeof (TMessage)));

            return consumer;
        }

        private static void _Subscribe(IDispatcherContext context, Consumes<TMessage>.For<TKey> consumer)
        {
            CorrelationIdDispatcher<TMessage, TKey> dispatcher = GetCorrelatedDispatcher(context, true);

            dispatcher.Attach(consumer);

            context.AddSubscription(new Subscription(typeof (TMessage).FullName, consumer.CorrelationId.ToString(), context.Bus.Endpoint.Uri));
        }

        private static void _Unsubscribe(IDispatcherContext context, Consumes<TMessage>.For<TKey> consumer)
        {
            CorrelationIdDispatcher<TMessage, TKey> dispatcher = GetCorrelatedDispatcher(context, false);
            if (dispatcher == null)
                return;

            dispatcher.Detach(consumer);

            if (dispatcher.Active(consumer.CorrelationId) == false)
            {
                context.RemoveSubscription(new Subscription(typeof (TMessage).FullName, consumer.CorrelationId.ToString(), context.Bus.Endpoint.Uri));

                // context.Detach(dispatcher);
            }
        }

        private static void GetComponentDispatcher(IDispatcherContext context)
        {
            IMessageDispatcher<TMessage> messageDispatcher = context.GetDispatcher<MessageDispatcher<TMessage>>();

            Type componentDispatcherType = typeof (SelectiveComponentDispatcher<,>).MakeGenericType(typeof (TComponent), typeof (TMessage));

            Consumes<TMessage>.Selected dispatcher = messageDispatcher.GetDispatcher<Consumes<TMessage>.Selected>(componentDispatcherType);

            if (dispatcher == null)
            {
                dispatcher = (Consumes<TMessage>.Selected) Activator.CreateInstance(componentDispatcherType, context);

                context.Attach(dispatcher);
                context.AddSubscription(new Subscription(typeof (TMessage), context.Bus.Endpoint.Uri));
            }
        }

        private static CorrelationIdDispatcher<TMessage, TKey> GetCorrelatedDispatcher(IDispatcherContext context, bool createIfNotFound)
        {
            IMessageDispatcher<TMessage> messageDispatcher = context.GetDispatcher<MessageDispatcher<TMessage>>();

            CorrelationIdDispatcher<TMessage, TKey> dispatcher = messageDispatcher.GetDispatcher<CorrelationIdDispatcher<TMessage, TKey>>();

            if (dispatcher == null && createIfNotFound)
            {
                dispatcher = new CorrelationIdDispatcher<TMessage, TKey>();

                context.Attach(dispatcher);
            }

            return dispatcher;
        }
    }
}