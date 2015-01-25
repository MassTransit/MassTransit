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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using MassTransit.Pipeline;
    using Pipeline;
    using Serialization;
    using Transports;


    public class RabbitMqPublishEndpoint :
        IPublishEndpoint
    {
        readonly ConcurrentDictionary<Type, Lazy<ISendEndpoint>> _cachedEndpoints;
        readonly IRabbitMqHost _host;
        readonly Uri _inputAddress;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly IMessageSerializer _serializer;

        public RabbitMqPublishEndpoint(IRabbitMqHost host, IMessageSerializer serializer, Uri inputAddress)
        {
            _host = host;
            _serializer = serializer;
            _inputAddress = inputAddress;
            _messageNameFormatter = host.MessageNameFormatter;
            _cachedEndpoints = new ConcurrentDictionary<Type, Lazy<ISendEndpoint>>();
        }

        async Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            ISendEndpoint endpoint = await GetEndpoint(typeof(T));
            await endpoint.Send(message, cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            ISendEndpoint endpoint = await GetEndpoint(typeof(T));
            await endpoint.Send(message, new PublishPipeContextAdapter<T>(publishPipe), cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            ISendEndpoint endpoint = await GetEndpoint(typeof(T));
            await endpoint.Send(message, new PublishPipeContextAdapter(publishPipe), cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Type messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Type messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            ISendEndpoint endpoint = await GetEndpoint(typeof(T));
            await endpoint.Send<T>(values, cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            ISendEndpoint endpoint = await GetEndpoint(typeof(T));
            await endpoint.Send(values, new PublishPipeContextAdapter<T>(publishPipe), cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            ISendEndpoint endpoint = await GetEndpoint(typeof(T));
            await endpoint.Send<T>(values, new PublishPipeContextAdapter(publishPipe), cancellationToken);
        }

        async Task<ISendEndpoint> GetEndpoint(Type messageType)
        {
            return _cachedEndpoints.GetOrAdd(messageType, x => new Lazy<ISendEndpoint>(() => CreateSendEndpoint(x))).Value;
        }

        ISendEndpoint CreateSendEndpoint(Type messageType)
        {
            SendSettings sendSettings = _host.GetSendSettings(messageType, _messageNameFormatter);

            ExchangeBindingSettings[] bindings = messageType.GetMessageTypes()
                .Select(type => type.GetExchangeBinding(_messageNameFormatter))
                .Where(binding => !sendSettings.ExchangeName.Equals(binding.Exchange.ExchangeName))
                .ToArray();

            Uri sendAddress = _host.Settings.GetSendAddress(sendSettings);

            var modelCache = new RabbitMqModelCache(_host.SendConnectionCache);

            var sendTransport = new RabbitMqSendTransport(modelCache, sendSettings, bindings);

            return new SendEndpoint(sendTransport, _serializer, sendAddress, _inputAddress);
        }
    }
}