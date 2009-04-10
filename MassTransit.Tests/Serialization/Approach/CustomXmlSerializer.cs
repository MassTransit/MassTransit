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
namespace MassTransit.Tests.Serialization.Approach
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Xml;
	using Magnum.Monads;

	public class CustomXmlSerializer
	{
		public byte[] Serialize<T>(T message)
		{
			using (MemoryStream output = new MemoryStream())
			{
				Serialize(output, message);

				return output.ToArray();
			}
		}

		public void Serialize<T>(Stream stream, T message)
		{
			var settings = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					Indent = true,
					NewLineOnAttributes = true,
				};

			using (var streamWriter = new StreamWriter(stream))
			using (var writer = XmlWriter.Create(streamWriter, settings))
			{
				SerializerContext context = new SerializerContext();

				foreach (K<Action<XmlWriter>> writerAction in context.Serialize(message).ToArray())
				{
					writerAction(x => x(writer));
				}

				writer.Close();
				streamWriter.Close();
			}
		}
	}
}