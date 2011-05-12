// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using Context;
	using Magnum.Threading;
	using Util;

	/// <summary>
	/// Serializes messages using the .NET Xml Serializer
	/// As such, limitations of that serializer apply to this one
	/// </summary>
	public class DotNotXmlMessageSerializer :
		IMessageSerializer
	{
		static readonly XmlAttributes _attributes;
		static readonly ReaderWriterLockedDictionary<Type, XmlSerializer> _deserializers;
		static readonly XmlSerializerNamespaces _namespaces;
		static readonly ReaderWriterLockedDictionary<Type, XmlSerializer> _serializers;

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

		public void Serialize<T>(Stream stream, ISendContext<T> context)
			where T : class
		{
			CheckConvention.EnsureSerializable(context.Message);
			XmlMessageEnvelope envelope = XmlMessageEnvelope.Create(context);

			GetSerializerFor<T>().Serialize(stream, envelope);
		}

		public object Deserialize(IReceiveContext context)
		{
			object obj = GetDeserializerFor(typeof (XmlReceiveMessageEnvelope)).Deserialize(context.BodyStream);
			if (obj.GetType() != typeof (XmlReceiveMessageEnvelope))
				throw new SerializationException("An unknown message type was received: " + obj.GetType().FullName);

			var envelope = (XmlReceiveMessageEnvelope) obj;

			if (string.IsNullOrEmpty(envelope.MessageType))
				throw new SerializationException("No message type found on envelope");

			Type t = Type.GetType(envelope.MessageType, true, true);

			using (var reader = new XmlNodeReader(envelope.Message))
			{
				obj = GetDeserializerFor(t).Deserialize(reader);
			}

			context.SetUsingMessageEnvelope(envelope);

			return obj;
		}

		static XmlSerializer GetSerializerFor<T>()
		{
			Type type = typeof (T);

			return _serializers.Retrieve(type, () => new XmlSerializer(typeof (XmlMessageEnvelope), new[] {type}));
		}

		static XmlSerializer GetDeserializerFor(Type type)
		{
			return _deserializers.Retrieve(type, () =>
				{
					var overrides = new XmlAttributeOverrides();
					overrides.Add(type, _attributes);

					return new XmlSerializer(type, overrides);
				});
		}
	}
}