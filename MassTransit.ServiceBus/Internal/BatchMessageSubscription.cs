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
    using Exceptions;
    using Subscriptions;

    public class BatchMessageSubscription<TComponent, TMessage, TBatchId> :
        ISubscriptionTypeInfo
        where TComponent : class, Consumes<Batch<TMessage, TBatchId>>.Selected
        where TMessage : class, BatchedBy<TBatchId>
    {
        public void Subscribe<T>(IDispatcherContext context, T component) where T : class
        {
            Consumes<Batch<TMessage, TBatchId>>.All consumer = GetTypedConsumer(component);

            _Subscribe(context, consumer);
        }

        public void Unsubscribe<T>(IDispatcherContext context, T component) where T : class
        {
            Consumes<Batch<TMessage, TBatchId>>.All consumer = GetTypedConsumer(component);

            _Unsubscribe(context, consumer);
        }

        public void AddComponent(IDispatcherContext context)
        {
            BatchDispatcher<TMessage, TBatchId> batchDispatcher = GetBatchDispatcher(context, true);

            Consumes<Batch<TMessage, TBatchId>>.Selected consumer = new SelectiveComponentDispatcher<TComponent, Batch<TMessage, TBatchId>>(context);

            batchDispatcher.Attach(consumer);
        }

        public void RemoveComponent(IDispatcherContext context)
        {
            throw new NotSupportedException("Removal of components is currently broken");
        }

        public void Dispose()
        {
        }

        private static Consumes<Batch<TMessage, TBatchId>>.All GetTypedConsumer<T>(T component)
        {
            Consumes<Batch<TMessage, TBatchId>>.All consumer = component as Consumes<Batch<TMessage, TBatchId>>.All;
            if (consumer == null)
                throw new ConventionException(string.Format("Object of type {0} does not consume messages of type {1}", typeof (T), typeof (TMessage)));

            return consumer;
        }

        private static void _Subscribe(IDispatcherContext context, Consumes<Batch<TMessage, TBatchId>>.All consumer)
        {
            BatchDispatcher<TMessage, TBatchId> batchDispatcher = GetBatchDispatcher(context, true);

            batchDispatcher.Attach(consumer);
        }

        private static BatchDispatcher<TMessage, TBatchId> GetBatchDispatcher(IDispatcherContext context, bool createIfNotFound)
        {
            IMessageDispatcher<TMessage> messageDispatcher = context.GetDispatcher<MessageDispatcher<TMessage>>();

            Type dispatcherType = typeof (BatchDispatcher<,>).MakeGenericType(typeof (TMessage), typeof (TBatchId));

            BatchDispatcher<TMessage, TBatchId> batchDispatcher = messageDispatcher.GetDispatcher<BatchDispatcher<TMessage, TBatchId>>(dispatcherType);

            if (batchDispatcher == null)
            {
                batchDispatcher = new BatchDispatcher<TMessage, TBatchId>(context);

                context.Attach(batchDispatcher);
                context.AddSubscription(new Subscription(typeof (TMessage).FullName, context.Bus.Endpoint.Uri));
            }

            return batchDispatcher;
        }

        private static void _Unsubscribe(IDispatcherContext context, Consumes<Batch<TMessage, TBatchId>>.All consumer)
        {
            BatchDispatcher<TMessage, TBatchId> batchDispatcher = GetBatchDispatcher(context, false);
            if (batchDispatcher == null)
                return;

            batchDispatcher.Detach(consumer);

            DetachIfInactive(context, batchDispatcher);
        }

        private static void DetachIfInactive(IDispatcherContext context, BatchDispatcher<TMessage, TBatchId> batchDispatcher)
        {
            if (batchDispatcher.Active == false)
            {
                context.Detach(batchDispatcher);

                if (context.GetDispatcher<MessageDispatcher<TMessage>>().Active == false)
                {
                    context.RemoveSubscription(new Subscription(typeof (TMessage).FullName, context.Bus.Endpoint.Uri));
                }
            }
        }
    }
}