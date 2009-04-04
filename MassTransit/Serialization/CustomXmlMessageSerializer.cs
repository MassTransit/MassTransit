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
	using System.IO;
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
			var settings = new XmlReaderSettings { IgnoreWhitespace = true};
			using (StreamReader streamReader = new StreamReader(stream))
			using (XmlReader reader = XmlReader.Create(streamReader, settings))
			{
				while (reader.Read() && reader.NodeType != XmlNodeType.Element)
				{
				}

				if (reader.EOF)
					throw new SerializationException("No document element found");

				object message = XmlObjectSerializer.GetSerializerFor(reader)
					.ReadObject(reader);

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
	}
}