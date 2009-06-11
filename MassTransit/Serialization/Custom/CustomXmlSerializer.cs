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
namespace MassTransit.Serialization.Custom
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Xml;
	using Magnum.Monads;

	public class CustomXmlSerializer :
		IXmlSerializer
	{
		private XmlReaderSettings _readerSettings;
		private XmlWriterSettings _writerSettings;

		public CustomXmlSerializer()
		{
			_writerSettings = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					Indent = true,
					NewLineOnAttributes = true,
					CloseOutput = false,
				};

			_readerSettings = new XmlReaderSettings
				{
					IgnoreWhitespace = true,
					CloseInput = false,
				};
		}

		public void Serialize<T>(Stream stream, T message)
			where T : class
		{
			using (var outputStream = new NonClosingStream(stream))
			using (var streamWriter = new StreamWriter(outputStream))
			using (var writer = XmlWriter.Create(streamWriter, _writerSettings))
			{
				SerializerContext context = new SerializerContext();

				foreach (K<Action<XmlWriter>> writerAction in context.Serialize(message).ToArray())
				{
					writerAction(x => x(writer));
				}
			}
		}

		public object Deserialize(Stream input)
		{
			using(var inputStream = new NonClosingStream(input))
			using(var streamReader = new StreamReader(inputStream))
			using (XmlReader reader = XmlReader.Create(streamReader, _readerSettings))
			{
				IDeserializerContext context = new DeserializerContext(reader);

				object message = context.Deserialize();

				if (message == null)
					throw new SerializationException("Could not deserialize message.");

				return message;
			}
		}

		public byte[] Serialize<T>(T message)
			where T : class
		{
			using (MemoryStream output = new MemoryStream())
			{
				Serialize(output, message);

				return output.ToArray();
			}
		}

		public object Deserialize(byte[] data)
		{
			using (MemoryStream input = new MemoryStream(data))
			{
				return Deserialize(input);
			}
		}
	}
}