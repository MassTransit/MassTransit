// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;
    using Magnum.Common.Threading;

    /// <summary>
    /// Serializes messages using the .NET Xml Serializer
    /// As such, limitations of that serializer apply to this one
    /// </summary>
    public class XmlMessageSerializer :
        IMessageSerializer
    {
        private static readonly XmlAttributes _attributes;
        private static readonly Dictionary<Type, XmlSerializer> _deserializers;
        private static readonly UpgradeableLock _lockContext;
        private static readonly XmlSerializerNamespaces _namespaces;
        private static readonly Dictionary<Type, XmlSerializer> _serializers;

        static XmlMessageSerializer()
        {
            _lockContext = new UpgradeableLock();
            _serializers = new Dictionary<Type, XmlSerializer>
                {
                };

            _deserializers = new Dictionary<Type, XmlSerializer>
                {
                    {typeof (XmlReceiveMessageEnvelope), new XmlSerializer(typeof (XmlReceiveMessageEnvelope))},
                };

            _namespaces = new XmlSerializerNamespaces();
            _namespaces.Add("", "");

            _attributes = new XmlAttributes();
            _attributes.XmlRoot = new XmlRootAttribute("Message");
        }

        public void Serialize<T>(Stream output, T message)
        {
            XmlMessageEnvelope envelope = new XmlMessageEnvelope(message);

            GetSerializerFor<T>().Serialize(output, envelope);
        }

        public object Deserialize(Stream input)
        {
            object obj = GetDeserializerFor(typeof (XmlReceiveMessageEnvelope)).Deserialize(input);
            if (obj.GetType() != typeof (XmlReceiveMessageEnvelope))
                throw new SerializationException("An unknown message type was received: " + obj.GetType().FullName);

            XmlReceiveMessageEnvelope envelope = (XmlReceiveMessageEnvelope) obj;

            if (string.IsNullOrEmpty(envelope.MessageType))
                throw new SerializationException("No message type found on envelope");

            Type t = Type.GetType(envelope.MessageType, true, true);

            using (var reader = new XmlNodeReader(envelope.Message))
            {
                obj = GetDeserializerFor(t).Deserialize(reader);
            }

            return obj;
        }

        private static XmlSerializer GetSerializerFor<T>()
        {
            Type type = typeof (T);

            using (var token = _lockContext.EnterUpgradableRead())
            {
                XmlSerializer serializer;
                if (_serializers.TryGetValue(type, out serializer))
                    return serializer;

                using (token.Upgrade())
                {
                    serializer = new XmlSerializer(typeof (XmlMessageEnvelope), new[] {type});

                    _serializers[type] = serializer;

                    return serializer;
                }
            }
        }

        private static XmlSerializer GetDeserializerFor(Type type)
        {
            using (var token = _lockContext.EnterUpgradableRead())
            {
                XmlSerializer serializer;
                if (_deserializers.TryGetValue(type, out serializer))
                    return serializer;

                using (token.Upgrade())
                {
                    XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                    overrides.Add(type, _attributes);

                    serializer = new XmlSerializer(type, overrides);

                    _deserializers[type] = serializer;

                    return serializer;
                }
            }
        }
    }
}