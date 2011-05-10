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
namespace MassTransit.Serialization.Custom
{
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Xml;

	public class CustomXmlSerializer :
		IXmlSerializer
	{
		readonly XmlReaderSettings _readerSettings;
		readonly XmlWriterSettings _writerSettings;

		public CustomXmlSerializer()
		{
			_writerSettings = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					Indent = true,
					NewLineOnAttributes = true,
					CloseOutput = false,
					CheckCharacters = false,
				};

			_readerSettings = new XmlReaderSettings
				{
					IgnoreWhitespace = true,
					CloseInput = false,
					CheckCharacters = false,
				};
		}

		public void Serialize<T>(Stream stream, T message)
			where T : class
		{
			using (var outputStream = new NonClosingStream(stream))
			{
				using (var streamWriter = new StreamWriter(outputStream))
				{
					using (XmlWriter writer = XmlWriter.Create(streamWriter, _writerSettings))
					{
						SerializeMessage(message, writer, new SerializerContext());
					}
				}
			}
		}

		public void Serialize<T>(Stream stream, T message, SerializerTypeMapper typeMapper)
			where T : class
		{
			using (var outputStream = new NonClosingStream(stream))
			{
				using (var streamWriter = new StreamWriter(outputStream))
				{
					using (XmlWriter writer = XmlWriter.Create(streamWriter, _writerSettings))
					{
						SerializeMessage(message, writer, new SerializerContext(typeMapper));
					}
				}
			}
		}

		public void Serialize<T>(TextWriter stream, T message)
			where T : class
		{
			using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
			{
				SerializeMessage(message, writer, new SerializerContext());
			}
		}

		public byte[] Serialize<T>(T message)
			where T : class
		{
			using (var output = new MemoryStream())
			{
				Serialize(output, message);

				return output.ToArray();
			}
		}

		public object Deserialize(Stream input)
		{
			using (var inputStream = new NonClosingStream(input))
			using (var streamReader = new StreamReader(inputStream))
			using (XmlReader reader = XmlReader.Create(streamReader, _readerSettings))
			{
				return DeserializeMessage(reader);
			}
		}

		public object Deserialize(TextReader textReader)
		{
			using (XmlReader reader = XmlReader.Create(textReader, _readerSettings))
			{
				return DeserializeMessage(reader);
			}
		}

		public object Deserialize(byte[] data)
		{
			using (var input = new MemoryStream(data))
			{
				return Deserialize(input);
			}
		}

		static void SerializeMessage<T>(T message, XmlWriter writer, SerializerContext context)
			where T : class
		{
			foreach (var writerAction in context.Serialize(message).ToArray())
			{
				writerAction(x => x(writer));
			}
		}

		static object DeserializeMessage(XmlReader reader)
		{
			IDeserializerContext context = new DeserializerContext(reader);

			object message = context.Deserialize();

			if (message == null)
				throw new SerializationException("Could not deserialize message.");

			return message;
		}
	}
}