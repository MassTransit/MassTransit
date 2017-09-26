// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if !NETCORE
    using Microsoft.ServiceBus.Messaging;
#else
    using Microsoft.Azure.ServiceBus;
#endif
    using Newtonsoft.Json.Linq;
    using Settings;
    using Transports;


    public static class ServiceBusTopicSubscriptionExtensions
    {
        public static IEnumerable<TopicSubscriptionSettings> GetTopicSubscription(this IMessageNameFormatter messageNameFormatter, Type messageType)
        {
            if (!IsSubscriptionMessageType(messageType))
                yield break;

            var temporary = IsTemporaryMessageType(messageType.GetTypeInfo());

            var topicDescription = Defaults.CreateTopicDescription(messageNameFormatter.GetMessageName(messageType).ToString());
            topicDescription.EnableExpress = temporary;

            var binding = new TopicSubscription(topicDescription);

            yield return binding;
        }

        static bool IsSubscriptionMessageType(Type messageType)
        {
            if (typeof(JToken) == messageType)
                return false;

            return true;
        }

        public static Uri GetTopicAddress(this IMessageNameFormatter messageNameFormatter, IServiceBusHost host, Type messageType)
        {
            var messageName = messageNameFormatter.GetMessageName(messageType).ToString();

            var builder = new UriBuilder(host.Settings.ServiceUri)
            {
                Path = messageName
            };

            return builder.Uri;
        }

        static bool IsTemporaryMessageType(this TypeInfo messageTypeInfo)
        {
            return (!messageTypeInfo.IsVisible && messageTypeInfo.IsClass)
                || (messageTypeInfo.IsGenericType && messageTypeInfo.GetGenericArguments().Any(x => IsTemporaryMessageType(x.GetTypeInfo())));
        }


        class TopicSubscription :
            TopicSubscriptionSettings
        {
            public TopicSubscription(TopicDescription topic)
            {
                Topic = topic;
            }

            public TopicDescription Topic { get; }

            protected bool Equals(TopicSubscription other)
            {
                return Topic.Path.Equals(other.Topic.Path);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((TopicSubscription)obj);
            }

            public override int GetHashCode()
            {
                return Topic.Path.GetHashCode();
            }
        }
    }
}