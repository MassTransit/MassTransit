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
    using PipeConfigurators;
    using Pipeline;
    using Pipeline.Pipes;
    using Serialization;
    using Transports;


    public abstract class BusBuilder
    {
        readonly Lazy<IMessageDeserializer> _deserializer;
        readonly IDictionary<string, Func<ISendEndpointProvider, IPublishEndpoint, IMessageDeserializer>> _deserializerFactories;
        readonly IList<IPipeSpecification<ConsumeContext>> _endpointPipeSpecifications;
        readonly IList<IReceiveEndpoint> _endpoints;
        readonly Lazy<IPublishEndpoint> _publishEndpointProvider;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly Lazy<IMessageSerializer> _serializer;
        Func<IMessageSerializer> _serializerFactory;

        protected BusBuilder(IEnumerable<IPipeSpecification<ConsumeContext>> endpointPipeSpecifications)
        {
            _endpoints = new List<IReceiveEndpoint>();
            _deserializer = new Lazy<IMessageDeserializer>(CreateDeserializer);
            _serializer = new Lazy<IMessageSerializer>(CreateSerializer);
            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpoint>(CreatePublishEndpoint);
            _deserializerFactories =
                new Dictionary<string, Func<ISendEndpointProvider, IPublishEndpoint, IMessageDeserializer>>(StringComparer.OrdinalIgnoreCase);

            _endpointPipeSpecifications = endpointPipeSpecifications.ToList();
            _serializerFactory = () => new JsonMessageSerializer();

            AddMessageDeserializer(JsonMessageSerializer.JsonContentType,
                (s, p) => new JsonMessageDeserializer(JsonMessageSerializer.Deserializer, s, p));
            AddMessageDeserializer(BsonMessageSerializer.BsonContentType,
                (s, p) => new BsonMessageDeserializer(BsonMessageSerializer.Deserializer, s, p));
            AddMessageDeserializer(XmlMessageSerializer.XmlContentType,
                (s, p) => new XmlMessageDeserializer(JsonMessageSerializer.Deserializer, s, p));
        }

        protected IEnumerable<IReceiveEndpoint> ReceiveEndpoints
        {
            get { return _endpoints; }
        }

        public IMessageSerializer MessageSerializer
        {
            get { return _serializer.Value; }
        }

        public IMessageDeserializer MessageDeserializer
        {
            get { return _deserializer.Value; }
        }

        protected ISendEndpointProvider SendEndpointProvider
        {
            get { return _sendEndpointProvider.Value; }
        }

        protected IPublishEndpoint PublishEndpoint
        {
            get { return _publishEndpointProvider.Value; }
        }

        public void AddMessageDeserializer(ContentType contentType,
            Func<ISendEndpointProvider, IPublishEndpoint, IMessageDeserializer> deserializerFactory)
        {
            if (contentType == null)
                throw new ArgumentNullException("contentType");
            if (deserializerFactory == null)
                throw new ArgumentNullException("deserializerFactory");

            if (_deserializer.IsValueCreated)
                throw new ConfigurationException("The deserializer has already been created, no additional deserializers can be added.");

            if (_deserializerFactories.ContainsKey(contentType.MediaType))
                return;

            _deserializerFactories[contentType.MediaType] = deserializerFactory;
        }

        public void SetMessageSerializer(Func<IMessageSerializer> serializerFactory)
        {
            if (serializerFactory == null)
                throw new ArgumentNullException("serializerFactory");

            if (_serializer.IsValueCreated)
                throw new ConfigurationException("The serializer has already been created, the serializer cannot be changed at this time.");

            _serializerFactory = serializerFactory;
        }

        public IConsumePipe CreateConsumePipe(IEnumerable<IPipeSpecification<ConsumeContext>> specifications)
        {
            return new ConsumePipe(_endpointPipeSpecifications.Concat(specifications));
        }

        IMessageSerializer CreateSerializer()
        {
            return _serializerFactory();
        }

        IMessageDeserializer CreateDeserializer()
        {
            IMessageDeserializer[] deserializers =
                _deserializerFactories.Values.Select(x => x(SendEndpointProvider, PublishEndpoint)).ToArray();

            return new SupportedMessageDeserializers(deserializers);
        }

        public void AddReceiveEndpoint(IReceiveEndpoint receiveEndpoint)
        {
            _endpoints.Add(receiveEndpoint);
        }

        protected abstract ISendEndpointProvider CreateSendEndpointProvider();

        protected abstract IPublishEndpoint CreatePublishEndpoint();
    }
}