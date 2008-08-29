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

    public class TypeInfo : ITypeInfo
    {
        private static readonly Type _batchType = typeof (Batch<,>);

        private readonly SubscriptionTypeInfo _subscriptionTypeInfo = new SubscriptionTypeInfo();
        private IPublicationTypeInfo _publicationTypeInfo;

        public TypeInfo(Type componentType)
        {
            Type publicationType = typeof (MessageTypePublication<>).MakeGenericType(componentType);

            _publicationTypeInfo = (IPublicationTypeInfo) Activator.CreateInstance(publicationType);
        }

        internal IPublicationTypeInfo PublicationTypeInfo
        {
            set { _publicationTypeInfo = value; }
        }

        public SubscriptionTypeInfo SubscriptionTypeInfo
        {
            get { return _subscriptionTypeInfo; }
        }

        public ISubscriptionTypeInfo GetSubscriptionTypeInfo()
        {
            return _subscriptionTypeInfo;
        }

        public IPublicationTypeInfo GetPublicationTypeInfo()
        {
            return _publicationTypeInfo;
        }

        public void Dispose()
        {
            _subscriptionTypeInfo.Dispose();
        }

        internal void AddCorrelatedSubscription(Type componentType, Type[] types)
        {
            Type subscriptionType = typeof (CorrelatedSubscription<,,>).MakeGenericType(componentType, types[0], types[1]);

            _subscriptionTypeInfo.AddSubscriber((ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType));
        }

        internal void AddMessageSubscription(Type componentType, Type[] types)
        {
            Type subscriptionType = typeof (MessageTypeSubscription<,>).MakeGenericType(componentType, types[0]);

            _subscriptionTypeInfo.AddSubscriber((ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType, SubscriptionMode.All));
        }

        internal void AddSelectiveSubscription(Type componentType, Type[] types)
        {
            Type messageType = types[0];

            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == _batchType)
            {
                AddBatchMessageSubscription(componentType, messageType);
            }
            else
            {
                AddMessageSubscription(componentType, messageType);
            }
        }

        internal void AddSagaSubscription(Type componentType, Type[] types)
        {
            Type subscriptionType = typeof (SagaSubscription<,>).MakeGenericType(componentType, types[0]);

            _subscriptionTypeInfo.AddSubscriber((ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType));
        }

        internal void SetPublicationType(Type componentType, Type[] types)
        {
            Type publicationType = typeof (CorrelatedPublication<,>).MakeGenericType(componentType, types[0]);

            _publicationTypeInfo = (IPublicationTypeInfo) Activator.CreateInstance(publicationType);
        }

        internal void AddStartSagaSubscription(Type componentType, Type[] types)
        {
            Type subscriptionType = typeof (StartSagaSubscription<,>).MakeGenericType(componentType, types[0]);

            _subscriptionTypeInfo.AddSubscriber((ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType));
        }

        private void AddMessageSubscription(Type componentType, Type messageType)
        {
            Type subscriptionType = typeof (MessageTypeSubscription<,>).MakeGenericType(componentType, messageType);

            _subscriptionTypeInfo.AddSubscriber((ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType, SubscriptionMode.Selected));
        }

        private void AddBatchMessageSubscription(Type componentType, Type messageType)
        {
            Type[] batchArguments = messageType.GetGenericArguments();

            Type subscriptionType = typeof (BatchMessageSubscription<,,>).MakeGenericType(componentType, batchArguments[0], batchArguments[1]);

            _subscriptionTypeInfo.AddSubscriber((ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType));
        }
    }
}