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
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.Reflection;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class JsonMessageTypeConverter :
		IMessageTypeConverter
	{
		readonly JsonSerializer _serializer;
		readonly IEnumerable<string> _supportedTypes;
		readonly JToken _token;
		readonly IDictionary<Type, object> _mapped;

		public JsonMessageTypeConverter(JsonSerializer serializer, JToken token, IEnumerable<string> supportedTypes)
		{
			_token = token;
			_supportedTypes = supportedTypes;
			_serializer = serializer;
			_mapped = new Dictionary<Type, object>();
		}

		public bool TryConvert<T>(out T message)
			where T : class
		{
			object existing;
			if (_mapped.TryGetValue(typeof(T), out existing))
			{
				message = (T) existing;
				return message != null;
			}

			string typeUrn = new MessageUrn(typeof (T)).ToString();

			if (_supportedTypes.Any(typeUrn.Equals))
			{
				object obj;
				if (typeof (T).IsInterface && typeof (T).IsAllowedMessageType())
				{
					Type proxyType = InterfaceImplementationBuilder.GetProxyFor(typeof (T));

					obj = FastActivator.Create(proxyType);

					UsingReader(jsonReader => _serializer.Populate(jsonReader, obj));
				}
				else
				{
					obj = FastActivator<T>.Create();

					UsingReader(jsonReader => _serializer.Populate(jsonReader, obj));
				}

				_mapped[typeof (T)] = obj;

				message = (T) obj;
				return true;
			}

			_mapped[typeof (T)] = null;

			message = null;
			return false;
		}

		void UsingReader(Action<JsonReader> callback)
		{
			if (_token == null)
				return;

			using (var jsonReader = new JTokenReader(_token))
			{
				callback(jsonReader);
			}
		}
	}
}