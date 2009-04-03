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
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Xml;
	using Internal;
	using log4net;

	public class CustomXmlMessageSerializer :
		IMessageSerializer
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (CustomXmlMessageSerializer));

		public CustomXmlMessageSerializer()
		{
			Namespace = "http://tempuri.org/";
		}

		/// <summary>
		/// The namespace to place in outgoing XML.
		/// </summary>
		public string Namespace { get; set; }

		public void Serialize<T>(Stream stream, T message)
		{
			var envelope = XmlMessageEnvelope.Create(message);

			var settings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true};

			using (var streamWriter = new StreamWriter(stream))
			using (var writer = XmlWriter.Create(streamWriter, settings))
			{
				var serializer = XmlObjectSerializer.GetSerializerForType(typeof (XmlMessageEnvelope));

				serializer.WriteObject(writer, null, "envelope", envelope, x =>
					{
						x.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
						x.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
					});

				writer.WriteEndDocument();
				writer.Close();
				streamWriter.Close();
			}
		}

		public object Deserialize(Stream stream)
		{
			using (StreamReader streamReader = new StreamReader(stream))
			using (XmlReader xmlReader = XmlReader.Create(streamReader))
			{
				var doc = new XmlDocument();
				doc.Load(xmlReader);

				var documentElement = doc.DocumentElement;
				if (documentElement == null)
					throw new InvalidOperationException("Unable to parse to XML document element");

				object message = Process(documentElement, null);
				if (message == null)
					throw new SerializationException("Could not deserialize message.");

				if (message is XmlMessageEnvelope)
				{
					XmlMessageEnvelope envelope = message as XmlMessageEnvelope;

					InboundMessageHeaders.SetCurrent(envelope.GetMessageHeadersSetAction());

					return envelope.Message;
				}

				return message;
			}
		}

		private object Process(XmlNode node, object parent)
		{
			XmlAttribute typeAttribute = node.Attributes["type", "http://www.w3.org/2001/XMLSchema-instance"];
			if (typeAttribute == null)
				throw new SerializationException("Unable to deserialize message body, no type information found");

			string typeName = typeAttribute.Value;
			string name = node.Name;

			if (parent != null)
			{
				if (parent is IEnumerable)
				{
					if (parent.GetType().IsArray)
						return XmlObjectSerializer.GetSerializerForType(parent.GetType().GetElementType()).ReadObject(node);

					var args = parent.GetType().GetGenericArguments();
					if (args.Length == 1)
						return XmlObjectSerializer.GetSerializerForType(args[0]).ReadObject(node);
				}

				PropertyInfo prop = parent.GetType().GetProperty(name);
				if (prop != null)
					return XmlObjectSerializer.GetSerializerForType(prop.PropertyType).ReadObject(node);
			}

			Type t = Type.GetType(typeName);
			if (t == null)
			{
				_log.Debug("Could not load " + typeName);

				throw new TypeLoadException("Could not handle type '" + typeName + "'.");
			}

			return XmlObjectSerializer.GetSerializerForType(t).ReadObject(node);
		}
	}
}