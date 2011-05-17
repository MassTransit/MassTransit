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
namespace MassTransit.Context
{
	using System.Collections.Generic;
	using System.IO;
	using Newtonsoft.Json;

	public class TransportMessageHeaders
	{
		static JsonSerializer _serializer;
		readonly IDictionary<string, string> _headers;

		public TransportMessageHeaders()
		{
			_headers = new Dictionary<string, string>();
		}

		TransportMessageHeaders(IDictionary<string, string> dictionary)
		{
			_headers = dictionary;
		}

		static JsonSerializer Serializer
		{
			get
			{
				if (_serializer != null)
					return _serializer;

				var settings = new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore,
						MissingMemberHandling = MissingMemberHandling.Ignore,
					};

				_serializer = JsonSerializer.Create(settings);

				return _serializer;
			}
		}

		public string this[string index]
		{
			get
			{
				string value;
				if (_headers.TryGetValue(index, out value))
					return value;

				return null;
			}
		}

		public void Add(string name, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				_headers.Remove(name);
				return;
			}

			_headers[name] = value;
		}

		public byte[] GetBytes()
		{
			if (_headers.Count == 0)
				return new byte[0];

			using (var stream = new MemoryStream())
			using (var streamWriter = new StreamWriter(stream))
			using (var jsonWriter = new JsonTextWriter(streamWriter))
			{
				jsonWriter.Formatting = Formatting.None;

				Serializer.Serialize(jsonWriter, _headers);
				jsonWriter.Flush();
				streamWriter.Flush();

				return stream.ToArray();
			}
		}

		public static TransportMessageHeaders Create(byte[] bytes)
		{
			if (bytes.Length == 0)
				return new TransportMessageHeaders();

			using (var stream = new MemoryStream(bytes, false))
			using (var streamReader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				var headers = Serializer.Deserialize<IDictionary<string, string>>(jsonReader);

				return new TransportMessageHeaders(headers);
			}
		}
	}
}