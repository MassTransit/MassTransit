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
	using System.Text;
	using Internal;
	using Magnum.Common;
	using Magnum.Common.Monads;
	using Newtonsoft.Json;

	public class JsonMessageSerializer :
		IMessageSerializer
	{
		public void Serialize<T>(Stream output, T message)
		{
			var envelope = new JsonMessageEnvelope(message);
			string wrappedJson = JavaScriptConvert.SerializeObject(envelope);

			using (var mstream = new MemoryStream(Encoding.UTF8.GetBytes(wrappedJson)))
			{
				mstream.WriteTo(output);
			}
		}

		public object Deserialize(Stream input)
		{
			string body = Encoding.UTF8.GetString(input.ReadToEnd());

			var envelope = JavaScriptConvert.DeserializeObject<JsonMessageEnvelope>(body);

			Type t = Type.GetType(envelope.MessageType, true, true);
			var message = JavaScriptConvert.DeserializeObject(envelope.Message, t);

			LocalContext.Current.InboundMessage(context => envelope.ApplyTo(context));

			return message;
		}
	}

	public static class ExtensionsToStream
	{
		public static byte[] ReadToEnd(this Stream stream)
		{
			using (MemoryStream content = new MemoryStream())
			{
				byte[] block = new byte[1024];

				int read = 0;
				while ((read = stream.Read(block, 0, 1024)) > 0)
				{
					content.Write(block, 0, read);
				}

				return content.ToArray();
			}
		}
	}
}