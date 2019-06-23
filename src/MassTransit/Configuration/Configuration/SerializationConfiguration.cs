// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using Serialization;


    public class SerializationConfiguration :
        ISerializationConfiguration
    {
        readonly Lazy<IMessageDeserializer> _deserializer;
        readonly IDictionary<string, DeserializerFactory> _deserializerFactories;
        readonly Lazy<IMessageSerializer> _serializer;
        readonly IDictionary<string, DeserializerFactory> _sourceDeserializerFactories;
        SerializerFactory _serializerFactory;

        public SerializationConfiguration()
        {
            _serializerFactory = () => new JsonMessageSerializer();
            _serializer = new Lazy<IMessageSerializer>(CreateSerializer);
            _deserializer = new Lazy<IMessageDeserializer>(CreateDeserializer);

            _deserializerFactories = new Dictionary<string, DeserializerFactory>(StringComparer.OrdinalIgnoreCase);

            AddDeserializer(JsonMessageSerializer.JsonContentType, () => new JsonMessageDeserializer(JsonMessageSerializer.Deserializer));
            AddDeserializer(BsonMessageSerializer.BsonContentType, () => new BsonMessageDeserializer(BsonMessageSerializer.Deserializer));
            AddDeserializer(XmlMessageSerializer.XmlContentType, () => new XmlMessageDeserializer(JsonMessageSerializer.Deserializer));
        }

        SerializationConfiguration(SerializationConfiguration source)
        {
            _serializerFactory = () => source.Serializer;
            _serializer = new Lazy<IMessageSerializer>(CreateSerializer);
            _deserializer = new Lazy<IMessageDeserializer>(CreateDeserializer);

            _sourceDeserializerFactories = source._deserializerFactories;
            _deserializerFactories = new Dictionary<string, DeserializerFactory>(StringComparer.OrdinalIgnoreCase);
        }

        public IMessageSerializer Serializer => _serializer.Value;
        public IMessageDeserializer Deserializer => _deserializer.Value;

        public void AddDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            if (deserializerFactory == null)
                throw new ArgumentNullException(nameof(deserializerFactory));

            _deserializerFactories[contentType.MediaType] = deserializerFactory;
        }

        public void SetSerializer(SerializerFactory serializerFactory)
        {
            if (serializerFactory == null)
                throw new ArgumentNullException(nameof(serializerFactory));

            if (_serializer.IsValueCreated)
                throw new ConfigurationException("The serializer has already been created, the serializer cannot be changed at this time.");

            _serializerFactory = serializerFactory;
        }

        public void ClearDeserializers()
        {
            _deserializerFactories.Clear();
        }

        public ISerializationConfiguration CreateSerializationConfiguration()
        {
            return new SerializationConfiguration(this);
        }

        IMessageSerializer CreateSerializer()
        {
            return _serializerFactory();
        }

        IMessageDeserializer CreateDeserializer()
        {
            if (_sourceDeserializerFactories != null)
            {
                foreach (KeyValuePair<string, DeserializerFactory> deserializerFactory in _sourceDeserializerFactories)
                {
                    if (!_deserializerFactories.ContainsKey(deserializerFactory.Key))
                        _deserializerFactories.Add(deserializerFactory);
                }
            }

            IMessageDeserializer[] deserializers = _deserializerFactories.Values.Select(x => x()).ToArray();

            return new SupportedMessageDeserializers(deserializers);
        }
    }
}
