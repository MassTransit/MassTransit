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

    public class MessageTypeSubscription<TComponent, TMessage> :
        ISubscriptionTypeInfo
        where TComponent : class, Consumes<TMessage>.All
        where TMessage : class
    {
        private readonly SubscriptionMode _mode;

        public MessageTypeSubscription(SubscriptionMode mode)
        {
            _mode = mode;
        }

        public void Dispose()
        {
        }

        public void Subscribe<T>(IDispatcherContext context, T component) where T : class
        {
            _Subscribe(context, GetConsumer(component));
        }

        public void Unsubscribe<T>(IDispatcherContext context, T component) where T : class
        {
            _Unsubscribe(context, GetConsumer(component));
        }

        public void AddComponent(IDispatcherContext context)
        {
            GetMessageDispatcher(context);
        }

        public void RemoveComponent(IDispatcherContext context)
        {
            throw new NotSupportedException("Removeal not working");
        }

        private void GetMessageDispatcher(IDispatcherContext context)
        {
            Type componentDispatcherType;

            if (_mode == SubscriptionMode.Selected)
                componentDispatcherType = typeof (SelectiveComponentDispatcher<,>).MakeGenericType(typeof (TComponent), typeof (TMessage));
            else
                componentDispatcherType = typeof (ComponentDispatcher<,>).MakeGenericType(typeof (TComponent), typeof (TMessage));

            Consumes<TMessage>.Selected dispatcher = context.GetDispatcher<Consumes<TMessage>.Selected>(componentDispatcherType);

            if (dispatcher == null)
            {
                dispatcher = (Consumes<TMessage>.Selected) Activator.CreateInstance(componentDispatcherType, context);

                context.Attach(dispatcher);
                context.AddSubscription(new Subscription(typeof (TMessage).FullName, context.Bus.Endpoint.Uri));
            }
        }

        private static Consumes<TMessage>.All GetConsumer<T>(T component)
        {
            Consumes<TMessage>.All consumer = component as Consumes<TMessage>.All;
            if (consumer == null)
                throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof (T), typeof (TMessage)));

            return consumer;
        }

        private static void _Subscribe(IDispatcherContext context, Consumes<TMessage>.All consumer)
        {
            context.Attach(consumer);
            context.AddSubscription(new Subscription(typeof (TMessage).FullName, context.Bus.Endpoint.Uri));
        }

        private static void _Unsubscribe(IDispatcherContext context, Consumes<TMessage>.All consumer)
        {
            context.Detach(consumer);

            if (context.GetDispatcher<MessageDispatcher<TMessage>>().Active == false)
            {
                context.RemoveSubscription(new Subscription(typeof (TMessage).FullName, context.Bus.Endpoint.Uri));
            }
        }
    }
}