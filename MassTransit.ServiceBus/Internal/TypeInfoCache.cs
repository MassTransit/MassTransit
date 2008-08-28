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
    using System.Collections.Generic;
    using Saga;

    public class TypeInfoCache : IDisposable
    {
        private static readonly Type _batchType = typeof (Batch<,>);
        private static readonly Type _consumerType = typeof (Consumes<>.All);
        private static readonly Type _orchestratesType = typeof(Orchestrates<>);
        private static readonly Type _startedByType = typeof(StartedBy<>);
        private static readonly Type _correlatedConsumerType = typeof (Consumes<>.For<>);
        private static readonly Type _correlatedMessageType = typeof (CorrelatedBy<>);
        private static readonly Type _saga = typeof (ISaga);
        private static readonly Type _selectiveConsumerType = typeof (Consumes<>.Selected);
        private readonly Dictionary<Type, TypeInfo> _types = new Dictionary<Type, TypeInfo>();

        public void Dispose()
        {
            foreach (ITypeInfo info in _types.Values)
            {
                info.Dispose();
            }
            _types.Clear();
        }

        public ITypeInfo Resolve<TComponent>()
        {
            return Resolve(typeof (TComponent));
        }

        public ITypeInfo Resolve(Type componentType)
        {
            lock (_types)
            {
                TypeInfo info;
                if (_types.TryGetValue(componentType, out info))
                    return info;

                info = new TypeInfo();

                List<Type> usedMessageTypes = new List<Type>();
                int publicationCount = 0;

                foreach (Type interfaceType in componentType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType)
                    {
                        if (interfaceType.GetGenericTypeDefinition() == _orchestratesType)
                        {
                            Type[] arguments = interfaceType.GetGenericArguments();

                            AddSagaSubscription(typeof(SagaSubscription<,>), info, usedMessageTypes, componentType, arguments[0]);
                        }
                        else if (interfaceType.GetGenericTypeDefinition() == _startedByType)
                        {
                            Type[] arguments = interfaceType.GetGenericArguments();

                            AddSagaSubscription(typeof (StartSagaSubscription<,>), info, usedMessageTypes, componentType, arguments[0]);
                        }
                        else if (interfaceType.GetGenericTypeDefinition() == _correlatedConsumerType)
                        {
                            Type[] arguments = interfaceType.GetGenericArguments();

                            if (usedMessageTypes.Contains(arguments[0]))
                                continue;

                            Type subscriptionType = typeof (CorrelatedSubscription<,,>).MakeGenericType(componentType, arguments[0], arguments[1]);

                            ISubscriptionTypeInfo subscriptionTypeInfo = (ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType);

                            info.SubscriptionTypeInfo.AddSubscriber(subscriptionTypeInfo);

                            usedMessageTypes.Add(arguments[0]);
                        }
                        else if (interfaceType.GetGenericTypeDefinition() == _selectiveConsumerType)
                        {
                            Type[] arguments = interfaceType.GetGenericArguments();

                            if (usedMessageTypes.Contains(arguments[0]))
                                continue;

                            Type messageType = arguments[0];

                            ISubscriptionTypeInfo subscriptionTypeInfo;

                            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == _batchType)
                            {
                                Type[] batchArguments = messageType.GetGenericArguments();

                                Type subscriptionType = typeof (BatchMessageSubscription<,,>).MakeGenericType(componentType, batchArguments[0], batchArguments[1]);

                                subscriptionTypeInfo = (ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType);
                            }
                            else
                            {
                                Type subscriptionType = typeof (MessageTypeSubscription<,>).MakeGenericType(componentType, messageType);

                                subscriptionTypeInfo = (ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType, SubscriptionMode.Selected);
                            }

                            info.SubscriptionTypeInfo.AddSubscriber(subscriptionTypeInfo);

                            usedMessageTypes.Add(arguments[0]);
                        }
                        else if (interfaceType.GetGenericTypeDefinition() == _consumerType)
                        {
                            Type[] arguments = interfaceType.GetGenericArguments();

                            if (usedMessageTypes.Contains(arguments[0]))
                                continue;

                            Type subscriptionType = typeof (MessageTypeSubscription<,>).MakeGenericType(componentType, arguments[0]);

                            ISubscriptionTypeInfo subscriptionTypeInfo = (ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType, SubscriptionMode.All);

                            info.SubscriptionTypeInfo.AddSubscriber(subscriptionTypeInfo);

                            usedMessageTypes.Add(arguments[0]);
                        }
                        else if (interfaceType.GetGenericTypeDefinition() == _correlatedMessageType)
                        {
                            Type[] arguments = interfaceType.GetGenericArguments();

                            Type publicationType = typeof (CorrelatedPublication<,>).MakeGenericType(componentType, arguments[0]);

                            IPublicationTypeInfo publicationTypeInfo = (IPublicationTypeInfo) Activator.CreateInstance(publicationType);

                            info.PublicationTypeInfo = publicationTypeInfo;

                            publicationCount++;
                        }
                    }
                    else
                    {
                        if (interfaceType == _saga)
                        {
                        }
                    }
                }

                if (publicationCount == 0)
                {
                    Type publicationType = typeof (MessageTypePublication<>).MakeGenericType(componentType);

                    IPublicationTypeInfo publicationTypeInfo = (IPublicationTypeInfo) Activator.CreateInstance(publicationType);

                    info.PublicationTypeInfo = publicationTypeInfo;
                }

                _types.Add(componentType, info);

                return info;
            }
        }

        private static void AddSagaSubscription(Type type, TypeInfo info, List<Type> usedMessageTypes, Type componentType, Type argument)
        {
            if (usedMessageTypes.Contains(argument))
                return;

            Type subscriptionType = type.MakeGenericType(componentType, argument);

            ISubscriptionTypeInfo subscriptionTypeInfo = (ISubscriptionTypeInfo) Activator.CreateInstance(subscriptionType);

            info.SubscriptionTypeInfo.AddSubscriber(subscriptionTypeInfo);

            usedMessageTypes.Add(argument);
        }

        public IPublicationTypeInfo GetPublicationTypeInfo<TComponent>() where TComponent : class
        {
            ITypeInfo typeInfo = Resolve<TComponent>();

            return typeInfo.GetPublicationTypeInfo();
        }

        public IPublicationTypeInfo GetPublicationTypeInfo(Type type)
        {
            ITypeInfo typeInfo = Resolve(type);

            return typeInfo.GetPublicationTypeInfo();
        }

        public ISubscriptionTypeInfo GetSubscriptionTypeInfo<TComponent>() where TComponent : class
        {
            ITypeInfo typeInfo = Resolve<TComponent>();

            return typeInfo.GetSubscriptionTypeInfo();
        }

        public ISubscriptionTypeInfo GetSubscriptionTypeInfo(Type type)
        {
            ITypeInfo typeInfo = Resolve(type);

            return typeInfo.GetSubscriptionTypeInfo();
        }
    }

    public enum SubscriptionMode
    {
        All,
        Selected,
        Correlated,
    }
}