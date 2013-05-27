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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using Util;


    public class SupportedMessageSerializers :
        ISupportedMessageSerializers
    {
        readonly IDictionary<string, IMessageSerializer> _serializers;
        Func<IMessageSerializer> _defaultSerializer;

        public SupportedMessageSerializers(params IMessageSerializer[] serializers)
        {
            _serializers = new Dictionary<string, IMessageSerializer>();
            _defaultSerializer = () =>
                {
                    var serializer = new VersionOneXmlMessageSerializer();

                    _defaultSerializer = () => serializer;

                    return serializer;
                };

            for (int i = 0; i < serializers.Length; i++)
                AddSerializer(serializers[i]);

            AddSerializer(new JsonMessageSerializer());
            AddSerializer(new BsonMessageSerializer());
            AddSerializer(new XmlMessageSerializer());
            AddSerializer(new BinaryMessageSerializer());
        }

        public bool TryGetSerializer(string contentType, out IMessageSerializer serializer)
        {
            if (!string.IsNullOrEmpty(contentType))
            {
                if (_serializers.TryGetValue(contentType, out serializer))
                    return true;
            }

            serializer = _defaultSerializer();
            return serializer != null;
        }

        public void AddSerializer([NotNull] IMessageSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            // so only one instance gets created, use it if it's found.
            if (serializer.ContentType == "application/vnd.masstransit+xmlv1")
                _defaultSerializer = () => serializer;

            if (_serializers.ContainsKey(serializer.ContentType))
                return;

            _serializers[serializer.ContentType] = serializer;
        }
    }
}