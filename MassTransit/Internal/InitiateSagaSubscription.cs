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
    using Saga;
    using Subscriptions;

    public class InitiateSagaSubscription<TSaga, TMessage> :
        ISubscriptionTypeInfo
        where TSaga : class, ISaga, Consumes<TMessage>.All
        where TMessage : class, CorrelatedBy<Guid>
    {
        public void Dispose()
        {
        }

        public void Subscribe<T>(IDispatcherContext context, T component) where T : class
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T>(IDispatcherContext context, T component) where T : class
        {
            throw new NotImplementedException();
        }

        public void AddComponent(IDispatcherContext context)
        {
            GetSagaDispatcher(context);
        }

        public void RemoveComponent(IDispatcherContext context)
        {
            throw new NotImplementedException("Components cannot be removed");
        }

        public void GetSagaDispatcher(IDispatcherContext context)
        {
            IMessageDispatcher<TMessage> messageDispatcher = context.GetDispatcher<MessageDispatcher<TMessage>>();

            InitiateSagaDispatcher<TSaga, TMessage> dispatcher = messageDispatcher.GetDispatcher<InitiateSagaDispatcher<TSaga, TMessage>>();

            if (dispatcher == null)
            {
                ISagaRepository<TSaga> repository = context.Builder.GetInstance<ISagaRepository<TSaga>>();

                dispatcher = new InitiateSagaDispatcher<TSaga, TMessage>(context.Bus, context.Builder, repository);

                context.Attach(dispatcher);
                context.AddSubscription(new Subscription(typeof (TMessage), context.Bus.Endpoint.Uri));
            }
        }
    }
}