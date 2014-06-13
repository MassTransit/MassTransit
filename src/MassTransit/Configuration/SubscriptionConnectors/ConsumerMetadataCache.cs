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
namespace MassTransit.SubscriptionConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;


    public class ConsumerMetadataCache<T> :
        IConsumerMetadataCache<T>
        where T : class
    {
        readonly MessageInterfaceType[] _consumerTypes;
        readonly MessageInterfaceType[] _contextConsumerTypes;
        readonly MessageInterfaceType[] _messageConsumerTypes;


        ConsumerMetadataCache()
        {
            _consumerTypes = GetConsumerMessageTypes().ToArray();
            _messageConsumerTypes = GetMessageConsumerTypes().ToArray();
            _contextConsumerTypes = GetContextConsumerTypes().ToArray();
        }

        public static MessageInterfaceType[] ConsumerTypes
        {
            get { return InstanceCache.Cached.Value.ConsumerTypes; }
        }

        public static MessageInterfaceType[] MessageConsumerTypes
        {
            get { return InstanceCache.Cached.Value.MessageConsumerTypes; }
        }

        public static MessageInterfaceType[] ContextConsumerTypes
        {
            get { return InstanceCache.Cached.Value.ContextConsumerTypes; }
        }

        MessageInterfaceType[] IConsumerMetadataCache<T>.ConsumerTypes
        {
            get { return _consumerTypes; }
        }

        MessageInterfaceType[] IConsumerMetadataCache<T>.MessageConsumerTypes
        {
            get { return _messageConsumerTypes; }
        }

        MessageInterfaceType[] IConsumerMetadataCache<T>.ContextConsumerTypes
        {
            get { return _contextConsumerTypes; }
        }

        static IEnumerable<MessageInterfaceType> GetConsumerMessageTypes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
                .Select(x => new MessageInterfaceType(x, x.GetGenericArguments()[0], typeof(T)))
                .Where(x => x.MessageType.IsValueType == false && x.MessageType != typeof(string));
        }


        static IEnumerable<MessageInterfaceType> GetMessageConsumerTypes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
                .Select(x => new MessageInterfaceType(x, x.GetGenericArguments()[0], typeof(T)))
                .Where(x => x.MessageType.IsValueType == false)
                .Where(IsNotContextType);
        }

        static IEnumerable<MessageInterfaceType> GetContextConsumerTypes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
                .Select(x => new MessageInterfaceType(x, x.GetGenericArguments()[0], typeof(T)))
                .Where(x => x.MessageType.IsValueType == false)
                .Where(type => !IsNotContextType(type));
        }

        static bool IsNotContextType(MessageInterfaceType x)
        {
            return !(x.MessageType.IsGenericType &&
                     x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>));
        }


        static class InstanceCache
        {
            internal static readonly Lazy<IConsumerMetadataCache<T>> Cached = new Lazy<IConsumerMetadataCache<T>>(
                () => new ConsumerMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}