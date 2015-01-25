// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Linq;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public static class ServiceBusTopicSubscriptionExtensions
    {
        public static TopicSubscriptionSettings GetTopicSubscription(this IMessageNameFormatter messageNameFormatter, Type messageType)
        {
            bool temporary = IsTemporaryMessageType(messageType);

            var topicDescription = new TopicDescription(messageNameFormatter.GetMessageName(messageType).ToString());

            var binding = new TopicSubscription(topicDescription);

            return binding;
        }

        public static Uri GetTopicAddress(this IMessageNameFormatter messageNameFormatter, IServiceBusHost host, Type messageType)
        {
            string messageName = messageNameFormatter.GetMessageName(messageType).ToString();

            var builder = new UriBuilder(host.Settings.ServiceUri)
            {
                Path = messageName
            };

            return builder.Uri;
        }

        public static bool IsTemporaryMessageType(this Type messageType)
        {
            return (!messageType.IsPublic && messageType.IsClass)
                || (messageType.IsGenericType && messageType.GetGenericArguments().Any(IsTemporaryMessageType));
        }


        class TopicSubscription :
            TopicSubscriptionSettings
        {
            readonly TopicDescription _topic;

            public TopicSubscription(TopicDescription topic)
            {
                _topic = topic;
            }

            public TopicDescription Topic
            {
                get { return _topic; }
            }
        }
    }
}