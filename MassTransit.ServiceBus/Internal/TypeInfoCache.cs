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
        #region Delegates

        public delegate void ActionDelegate(TypeInfo info, Type componentType, Type[] types);

        #endregion

        private static readonly IList<ActionEntry> _consumerActions = new List<ActionEntry>();

        private static readonly Type _consumerType = typeof (Consumes<>.All);
        private static readonly Type _correlatedConsumerType = typeof (Consumes<>.For<>);
        private static readonly Type _correlatedMessageType = typeof (CorrelatedBy<>);
        private static readonly Type _orchestratesType = typeof (Orchestrates<>);
        private static readonly Type _selectiveConsumerType = typeof (Consumes<>.Selected);
        private static readonly Type _startedByType = typeof (StartedBy<>);
        private readonly Dictionary<Type, TypeInfo> _types = new Dictionary<Type, TypeInfo>();

        static TypeInfoCache()
        {
            _consumerActions.Add(new ActionEntry(_startedByType, (i, c, t) => i.AddStartSagaSubscription(c, t)));
            _consumerActions.Add(new ActionEntry(_orchestratesType, (i, c, t) => i.AddSagaSubscription(c, t)));
            _consumerActions.Add(new ActionEntry(_correlatedConsumerType, (i, c, t) => i.AddCorrelatedSubscription(c, t)));
            _consumerActions.Add(new ActionEntry(_selectiveConsumerType, (i, c, t) => i.AddSelectiveSubscription(c, t)));
            _consumerActions.Add(new ActionEntry(_consumerType, (i, c, t) => i.AddMessageSubscription(c, t)));
            _consumerActions.Add(new ActionEntry(_correlatedMessageType, (i, c, t) => i.SetPublicationType(c, t)));
        }

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

                info = new TypeInfo(componentType);

                List<Type> usedMessageTypes = new List<Type>();

                foreach (Type interfaceType in componentType.GetInterfaces())
                {
                    if (!interfaceType.IsGenericType) continue;

                    Type genericType = interfaceType.GetGenericTypeDefinition();
                    Type[] types = interfaceType.GetGenericArguments();

                    if (usedMessageTypes.Contains(types[0])) continue; 

                    foreach (ActionEntry entry in _consumerActions)
                    {
                        if (entry.GenericType != genericType) continue;

                        usedMessageTypes.Add(types[0]);

                        entry.AddAction(info, componentType, types);
                        break;
                    }
                }

                _types.Add(componentType, info);

                return info;
            }
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

        public class ActionEntry
        {
            private readonly ActionDelegate _addAction;
            private readonly Type _genericType;

            public ActionEntry(Type genericType, ActionDelegate addAction)
            {
                _genericType = genericType;
                _addAction = addAction;
            }

            public Type GenericType
            {
                get { return _genericType; }
            }

            public ActionDelegate AddAction
            {
                get { return _addAction; }
            }
        }
    }

    public enum SubscriptionMode
    {
        All,
        Selected,
        Correlated,
    }
}