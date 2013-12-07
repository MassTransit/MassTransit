// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Logging;
    using Magnum;
    using Magnum.Extensions;
    using Subscriptions.Coordinator;
    using Subscriptions.Messages;


    public class RabbitMqSubscriptionBinder :
        SubscriptionObserver
    {
        static readonly ILog _log = Logger.Get(typeof(RabbitMqSubscriptionBinder));
        readonly Dictionary<Guid, MessageName> _bindings;
        readonly InboundRabbitMqTransport _inboundTransport;
        readonly IRabbitMqEndpointAddress _inputAddress;
        readonly IMessageNameFormatter _messageNameFormatter;

        public RabbitMqSubscriptionBinder(IServiceBus bus)
        {
            _bindings = new Dictionary<Guid, MessageName>();

            _inboundTransport = bus.Endpoint.InboundTransport as InboundRabbitMqTransport;
            if (_inboundTransport == null)
            {
                throw new ConfigurationException(
                    "The bus must be receiving from a RabbitMQ endpoint for this interceptor to work");
            }

            _inputAddress = _inboundTransport.Address.CastAs<IRabbitMqEndpointAddress>();

            _messageNameFormatter = _inboundTransport.MessageNameFormatter;
        }

        public void OnSubscriptionAdded(SubscriptionAdded message)
        {
            Guard.AgainstNull(_inputAddress, "InputAddress", "The input address was not set");

            Type messageType = Type.GetType(message.MessageName);
            if (messageType == null)
            {
                _log.InfoFormat("Unknown message type '{0}', unable to add subscription", message.MessageName);
                return;
            }

            MessageName messageName = _messageNameFormatter.GetMessageName(messageType);

            _inboundTransport.BindSubscriberExchange(RabbitMqEndpointAddress.Parse(message.EndpointUri),
                messageName.ToString(), IsTemporaryMessageType(messageType));

            _bindings[message.SubscriptionId] = messageName;
        }

        public void OnSubscriptionRemoved(SubscriptionRemoved message)
        {
            Guard.AgainstNull(_inputAddress, "InputAddress", "The input address was not set");

            MessageName messageName;
            if (_bindings.TryGetValue(message.SubscriptionId, out messageName))
            {
                _inboundTransport.UnbindSubscriberExchange(messageName.ToString());

                _bindings.Remove(message.SubscriptionId);
            }
        }

        public void OnComplete()
        {
        }

        static bool IsTemporaryMessageType(Type messageType)
        {
            return (!messageType.IsPublic && messageType.IsClass)
                   || (messageType.IsGenericType
                       && messageType.GetGenericArguments().Any(x => IsTemporaryMessageType(x)));
        }
    }
}