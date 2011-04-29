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
namespace MassTransit.Serialization
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Xml;
	using System.Xml.Serialization;
	using Magnum.Threading;
	using MessageHeaders;
	using Util;

	/// <summary>
	/// Serializes messages using the .NET Xml Serializer
	/// As such, limitations of that serializer apply to this one
	/// </summary>
	public class DotNotXmlMessageSerializer :
		IMessageSerializer
	{
		private static readonly XmlAttributes _attributes;
		private static readonly ReaderWriterLockedDictionary<Type, XmlSerializer> _deserializers;
		private static readonly XmlSerializerNamespaces _namespaces;
		private static readonly ReaderWriterLockedDictionary<Type, XmlSerializer> _serializers;

		static DotNotXmlMessageSerializer()
		{
			_serializers = new ReaderWriterLockedDictionary<Type, XmlSerializer>();

			_deserializers = new ReaderWriterLockedDictionary<Type, XmlSerializer>
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
			CheckConvention.EnsureSerializable(message);
			var envelope = XmlMessageEnvelope.Create(message);

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

			InboundMessageHeaders.SetCurrent(envelope.GetMessageHeadersSetAction());

			return obj;
		}

		private static XmlSerializer GetSerializerFor<T>()
		{
			Type type = typeof (T);

			return _serializers.Retrieve(type, () => new XmlSerializer(typeof (XmlMessageEnvelope), new[] {type}));
		}

		private static XmlSerializer GetDeserializerFor(Type type)
		{
			return _deserializers.Retrieve(type, () =>
				{
					XmlAttributeOverrides overrides = new XmlAttributeOverrides();
					overrides.Add(type, _attributes);

					return new XmlSerializer(type, overrides);
				});
		}
	}
}