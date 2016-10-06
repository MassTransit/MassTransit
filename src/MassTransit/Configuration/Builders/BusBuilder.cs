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
namespace MassTransit.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using BusConfigurators;
    using GreenPipes;
    using Pipeline;
    using Serialization;
    using Transports;
    using Util;


    public abstract class BusBuilder :
        IBusBuilder
    {
        readonly BusObservable _busObservable;
        readonly Lazy<IConsumePipe> _consumePipe;
        readonly IConsumePipeFactory _consumePipeFactory;
        readonly ISendPipeFactory _sendPipeFactory;
        readonly IPublishPipeFactory _publishPipeFactory;
        readonly IDictionary<string, DeserializerFactory> _deserializerFactories;
        readonly IBusHostControl[] _hosts;
        readonly Lazy<Uri> _inputAddress;
        readonly IDictionary<string, IReceiveEndpoint> _receiveEndpoints;
        readonly Lazy<ISendTransportProvider> _sendTransportProvider;
        readonly Lazy<IMessageSerializer> _serializer;
        Func<IMessageSerializer> _serializerFactory;

        protected BusBuilder(IConsumePipeFactory consumePipeFactory, ISendPipeFactory sendPipeFactory,
            IPublishPipeFactory publishPipeFactory, IEnumerable<IBusHostControl> hosts)
        {
            _consumePipeFactory = consumePipeFactory;
            _sendPipeFactory = sendPipeFactory;
            _publishPipeFactory = publishPipeFactory;
            _hosts = hosts.ToArray();

            _deserializerFactories = new Dictionary<string, DeserializerFactory>(StringComparer.OrdinalIgnoreCase);
            _receiveEndpoints = new Dictionary<string, IReceiveEndpoint>();
            _serializerFactory = () => new JsonMessageSerializer();
            _busObservable = new BusObservable();
            _serializer = new Lazy<IMessageSerializer>(CreateSerializer);
            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);

            _inputAddress = new Lazy<Uri>(GetInputAddress);
            _consumePipe = new Lazy<IConsumePipe>(GetConsumePipe);

            AddMessageDeserializer(JsonMessageSerializer.JsonContentType,
                (s, p) => new JsonMessageDeserializer(JsonMessageSerializer.Deserializer, s, p));
            AddMessageDeserializer(BsonMessageSerializer.BsonContentType,
                (s, p) => new BsonMessageDeserializer(BsonMessageSerializer.Deserializer, s, p));
            AddMessageDeserializer(XmlMessageSerializer.XmlContentType,
                (s, p) => new XmlMessageDeserializer(JsonMessageSerializer.Deserializer, s, p));
        }

        protected BusObservable BusObservable => _busObservable;

        protected IEnumerable<IReceiveEndpoint> ReceiveEndpoints => _receiveEndpoints.Values;

        public IMessageSerializer MessageSerializer => _serializer.Value;

        public ISendTransportProvider SendTransportProvider => _sendTransportProvider.Value;

        protected Uri InputAddress => _inputAddress.Value;

        protected IConsumePipe ConsumePipe => _consumePipe.Value;

        protected abstract Uri GetInputAddress();
        protected abstract IConsumePipe GetConsumePipe();

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            if (deserializerFactory == null)
                throw new ArgumentNullException(nameof(deserializerFactory));

            if (_deserializerFactories.ContainsKey(contentType.MediaType))
                return;

            _deserializerFactories[contentType.MediaType] = deserializerFactory;
        }

        public void SetMessageSerializer(Func<IMessageSerializer> serializerFactory)
        {
            if (serializerFactory == null)
                throw new ArgumentNullException(nameof(serializerFactory));

            if (_serializer.IsValueCreated)
                throw new ConfigurationException("The serializer has already been created, the serializer cannot be changed at this time.");

            _serializerFactory = serializerFactory;
        }

        public ISendPipe CreateSendPipe(params ISendPipeSpecification[] specifications)
        {
            return _sendPipeFactory.CreateSendPipe(specifications);
        }

        public IPublishPipe CreatePublishPipe(params IPublishPipeSpecification[] specifications)
        {
            return _publishPipeFactory.CreatePublishPipe(specifications);
        }

        public IConsumePipe CreateConsumePipe(params IConsumePipeSpecification[] specifications)
        {
            return _consumePipeFactory.CreateConsumePipe(specifications);
        }

        IMessageSerializer CreateSerializer()
        {
            return _serializerFactory();
        }

        public IMessageDeserializer GetMessageDeserializer(ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider)
        {
            IMessageDeserializer[] deserializers =
                _deserializerFactories.Values.Select(x => x(sendEndpointProvider, publishEndpointProvider)).ToArray();

            return new SupportedMessageDeserializers(deserializers);
        }

        public void AddReceiveEndpoint(string endpointKey, IReceiveEndpoint receiveEndpoint)
        {
            if (endpointKey == null)
                throw new ArgumentNullException(nameof(endpointKey));

            if (_receiveEndpoints.ContainsKey(endpointKey))
                throw new ConfigurationException($"A receive endpoint with the same key was already added: {endpointKey}");

            _receiveEndpoints.Add(endpointKey, receiveEndpoint);
        }

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _busObservable.Connect(observer);
        }

        public IBusControl Build()
        {
            try
            {
                PreBuild();

                var sendEndpointProvider = CreateSendEndpointProvider(InputAddress);
                var publishEndpointProvider = CreatePublishEndpointProvider(InputAddress);

                var bus = new MassTransitBus(InputAddress, ConsumePipe, sendEndpointProvider, publishEndpointProvider, ReceiveEndpoints, _hosts, BusObservable);

                TaskUtil.Await(() => _busObservable.PostCreate(bus));

                return bus;
            }
            catch (Exception exception)
            {
                TaskUtil.Await(() => BusObservable.CreateFaulted(exception));

                throw;
            }
        }

        protected virtual void PreBuild()
        {
        }

        protected abstract ISendTransportProvider CreateSendTransportProvider();

        public abstract ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications);

        public abstract IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications);
    }
}