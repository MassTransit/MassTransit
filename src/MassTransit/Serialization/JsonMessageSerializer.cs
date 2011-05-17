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
	using System.Collections.Generic;
	using System.IO;
	using Context;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class JsonMessageSerializer :
		IMessageSerializer
	{
		const string ContentTypeHeaderValue = "application/vnd.masstransit+json";

		readonly JsonSerializerSettings _settings;
		JsonSerializer _serializer;

		public JsonMessageSerializer()
		{
			_settings = new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore,
					DefaultValueHandling = DefaultValueHandling.Ignore,
					MissingMemberHandling = MissingMemberHandling.Ignore,
					ObjectCreationHandling = ObjectCreationHandling.Auto,
					ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
					ContractResolver = new JsonContractResolver(),
					Converters = new List<JsonConverter>(new[] {new InterfaceProxyConverter()})
				};

			_serializer = JsonSerializer.Create(_settings);
		}

		public string ContentType
		{
			get { return ContentTypeHeaderValue; }
		}

		public void Serialize<T>(Stream output, ISendContext<T> context)
			where T : class
		{
			context.SetContentType(ContentTypeHeaderValue);

			Envelope envelope = Envelope.Create(context);

			using (var writer = new StreamWriter(output))
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				jsonWriter.Formatting = Formatting.Indented;

				_serializer.Serialize(jsonWriter, envelope);

				jsonWriter.Flush();
				writer.Flush();
			}
		}

		public void Deserialize(IReceiveContext context)
		{
			Envelope result;
			using (var reader = new StreamReader(context.BodyStream))
			using (var jsonReader = new JsonTextReader(reader))
			{
				result = _serializer.Deserialize<Envelope>(jsonReader);
			}

			context.SetUsingEnvelope(result);
			context.SetMessageTypeConverter(new JsonMessageTypeConverter(_serializer, result.Message as JToken,
				result.MessageType));
		}
	}
}