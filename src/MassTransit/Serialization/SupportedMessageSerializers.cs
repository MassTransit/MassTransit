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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;


    public class SupportedMessageSerializers :
        ISupportedMessageSerializers
    {
        readonly HashSet<Type> _serializerTypes;
        readonly IDictionary<string, IMessageSerializer> _serializers;
        Func<IMessageSerializer> _defaultSerializer;


        public SupportedMessageSerializers(params IMessageSerializer[] serializers)
        {
            _serializers = new Dictionary<string, IMessageSerializer>(StringComparer.InvariantCultureIgnoreCase);
            _serializerTypes = new HashSet<Type>();

            for (int i = 0; i < serializers.Length; i++)
                AddSerializer(serializers[i]);

            AddSerializer<JsonMessageSerializer>();
            AddSerializer<BsonMessageSerializer>();
            AddSerializer<BinaryMessageSerializer>();

            _defaultSerializer = () =>
            {
                IMessageSerializer serializer;
                if (!TryGetSerializer(JsonMessageSerializer.JsonContentType, out serializer))
                    serializer = new JsonMessageSerializer();

                _defaultSerializer = () => serializer;

                return serializer;
            };
        }

        public bool TryGetSerializer(ContentType contentType, out IMessageSerializer serializer)
        {
            if (contentType == null || string.IsNullOrWhiteSpace(contentType.MediaType))
            {
                serializer = _defaultSerializer();
                return _defaultSerializer != null;
            }

            if (_serializers.TryGetValue(contentType.MediaType, out serializer))
                return true;

            serializer = _defaultSerializer();
            return serializer != null;
        }

        public void AddSerializer<T>()
            where T : IMessageSerializer, new()
        {
            if (_serializerTypes.Contains(typeof(T)))
                return;

            AddSerializer(new T());
        }

        public void AddSerializer(IMessageSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            if (_serializers.ContainsKey(serializer.ContentType))
                return;

            _serializers[serializer.ContentType] = serializer;
        }
    }
}