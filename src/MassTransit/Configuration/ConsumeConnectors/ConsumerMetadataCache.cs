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
namespace MassTransit.ConsumeConnectors
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
        readonly MessageInterfaceType[] _messageConsumerTypes;

        ConsumerMetadataCache()
        {
            _consumerTypes = GetConsumerMessageTypes().ToArray();
            _messageConsumerTypes = GetMessageConsumerTypes().ToArray();
        }

        public static MessageInterfaceType[] ConsumerTypes
        {
            get { return Cached.Metadata.Value.ConsumerTypes; }
        }

        public static MessageInterfaceType[] MessageConsumerTypes
        {
            get { return Cached.Metadata.Value.MessageConsumerTypes; }
        }

        MessageInterfaceType[] IConsumerMetadataCache<T>.ConsumerTypes
        {
            get { return _consumerTypes; }
        }

        MessageInterfaceType[] IConsumerMetadataCache<T>.MessageConsumerTypes
        {
            get { return _messageConsumerTypes; }
        }

        static IEnumerable<MessageInterfaceType> GetConsumerMessageTypes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
                .Select(x => new MessageInterfaceType(x.GetGenericArguments()[0], typeof(T)))
                .Where(x => x.MessageType.IsValueType == false && x.MessageType != typeof(string));
        }

        static IEnumerable<MessageInterfaceType> GetMessageConsumerTypes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
                .Select(x => new MessageInterfaceType(x.GetGenericArguments()[0], typeof(T)))
                .Where(x => x.MessageType.IsValueType == false);
        }


        static class Cached
        {
            internal static readonly Lazy<IConsumerMetadataCache<T>> Metadata = new Lazy<IConsumerMetadataCache<T>>(
                () => new ConsumerMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}