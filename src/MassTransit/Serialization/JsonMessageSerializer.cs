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
	using System.Text;
	using Context;
	using Magnum.Extensions;
	using Newtonsoft.Json;

	public class JsonMessageSerializer :
		IMessageSerializer
	{
		readonly JsonSerializerSettings _settings;

		public JsonMessageSerializer()
		{
			_settings = new JsonSerializerSettings
				{
					MissingMemberHandling = MissingMemberHandling.Ignore,
					NullValueHandling = NullValueHandling.Ignore,
					ObjectCreationHandling = ObjectCreationHandling.Auto,
					DefaultValueHandling = DefaultValueHandling.Ignore
				};
		}

		public void Serialize<T>(Stream output, ISendContext<T> context) 
			where T : class
		{
			JsonMessageEnvelope envelope = JsonMessageEnvelope.Create(context);

			string strOut = JsonConvert.SerializeObject(envelope, Formatting.Indented, _settings);
			byte[] buff = Encoding.UTF8.GetBytes(strOut);

			output.Write(buff, 0, buff.Length);
		}

		public object Deserialize(IReceiveContext context)
		{
			string text = context.BodyStream.ReadToEndAsText();

			var envelope = JsonConvert.DeserializeObject<JsonMessageEnvelope>(text, _settings);

			context.SetUsingMessageEnvelope(envelope);

			Type messageType = Type.GetType(envelope.MessageType, false, true);
			if (messageType == null)
				return envelope.Message;

			object obj = JsonConvert.DeserializeObject(envelope.Message.ToString(), messageType);
			
			return obj;
		}
	}
}