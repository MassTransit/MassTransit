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
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using Pipeline.Configuration;
    using Pipeline.Sinks;
    using Util;

    /// <summary>
    /// Makes sure that the exchange for the published message is available. This ensures
    /// that we'll never get 404 exchange not found for published messages. If someone is
    /// listening to them; that's another question (there might be no queue bound to it).
    /// </summary>
    public class PublishEndpointInterceptor :
        IOutboundMessageInterceptor
    {
        readonly IDictionary<Type, UnsubscribeAction> _added;
        readonly IRabbitMqEndpointAddress _address;
        readonly IServiceBus _bus;
        readonly InboundRabbitMqTransport _inboundTransport;
        readonly IMessageNameFormatter _messageNameFormatter;

        public PublishEndpointInterceptor(IServiceBus bus)
        {
            _bus = bus;

            _inboundTransport = _bus.Endpoint.InboundTransport as InboundRabbitMqTransport;
            if (_inboundTransport == null)
                throw new ConfigurationException(
                    "The bus must be receiving from a RabbitMQ endpoint for this interceptor to work");

            _messageNameFormatter = _inboundTransport.MessageNameFormatter;

            _address = _inboundTransport.Address.CastAs<IRabbitMqEndpointAddress>();

            _added = new Dictionary<Type, UnsubscribeAction>();
        }

        public void PreDispatch(ISendContext context)
        {
            lock (_added)
            {
                Type messageType = context.DeclaringMessageType;

                if (_added.ContainsKey(messageType))
                    return;

                AddEndpointForType(messageType);
            }
        }

        public void PostDispatch(ISendContext context)
        {
        }

        void AddEndpointForType(Type messageType)
        {
            IEnumerable<Type> types = _inboundTransport.BindExchangesForPublisher(messageType, _messageNameFormatter);
            foreach (Type type in types)
            {
                if (_added.ContainsKey(type))
                    continue;

                MessageName messageName = _messageNameFormatter.GetMessageName(type);

                IRabbitMqEndpointAddress messageEndpointAddress = _address.ForQueue(messageName.ToString());

                FindOrAddEndpoint(type, messageEndpointAddress);
            }
        }

        /// <summary>
        /// Finds all endpoints in the outbound pipeline and starts routing messages
        /// to that endpoint.
        /// </summary>
        /// <param name="messageType">type of message</param>
        /// <param name="address">The message endpoint address.</param>
        void FindOrAddEndpoint(Type messageType, IRabbitMqEndpointAddress address)
        {
            var locator = new PublishEndpointSinkLocator(messageType, address);
            _bus.OutboundPipeline.Inspect(locator);

            if (locator.Found) // there was already a subscribed endpoint
            {
                _added.Add(messageType, () => true);
                return;
            }

            IEndpoint endpoint = _bus.GetEndpoint(address.Uri);

            // otherwise, create the sink for this message type and connect the out
            // bound pipeline to this sink.
            this.FastInvoke(new[] {messageType}, "CreateEndpointSink", endpoint);
        }

        [UsedImplicitly]
        void CreateEndpointSink<TMessage>(IEndpoint endpoint)
            where TMessage : class
        {
            var endpointSink = new EndpointMessageSink<TMessage>(endpoint);

            var filterSink = new OutboundMessageFilter<TMessage>(endpointSink,
                context => context.DeclaringMessageType == typeof(TMessage));

            UnsubscribeAction unsubscribeAction = _bus.OutboundPipeline.ConnectToRouter(filterSink);

            _added.Add(typeof(TMessage), unsubscribeAction);
        }
    }
}