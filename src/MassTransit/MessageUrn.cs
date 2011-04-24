    
// Copyright 2010 Chris Patterson
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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable]
	public class MessageUrn :
		Uri
	{
		[ThreadStatic]
		static IDictionary<Type, string> _cache;

		public MessageUrn(Type type)
			: base(GetUrnForType(type))
		{
		}

		public MessageUrn(string uriString)
			: base(uriString)
		{
		}

		protected MessageUrn(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		public Type GetType(bool throwOnError = true, bool ignoreCase = true)
		{
			if (Segments.Length == 0)
				return null;

			string[] names = Segments[0].Split(':');
			if (names[0] != "message")
				return null;

			string typeName;

			if (names.Length == 2)
				typeName = names[1];
			else if (names.Length == 3)
				typeName = names[1] + "." + names[2] + ", " + names[1];
			else if (names.Length >= 4)
				typeName = names[1] + "." + names[2] + ", " + names[3];
			else
				return null;

			Type messageType = Type.GetType(typeName, true, true);

			return messageType;
		}

		static string IsInCache(Type type, Func<Type, string> provider)
		{
			if (_cache == null)
				_cache = new Dictionary<Type, string>();

			string urn;
			if (_cache.TryGetValue(type, out urn))
				return urn;

			urn = provider(type);

			_cache[type] = urn;

			return urn;
		}

		static string GetUrnForType(Type type)
		{
			return IsInCache(type, x =>
				{
					var sb = new StringBuilder("urn:message:");

					return GetMessageName(sb, type, true);
				});
		}

		static string GetMessageName(StringBuilder sb, Type type, bool includeScope)
		{
            if (type.IsGenericParameter)
                return "";

			if (includeScope && type.Namespace != null)
			{
				string ns = type.Namespace;
				sb.Append(ns);

				sb.Append(':');
			}

			if (type.IsNested)
			{
				GetMessageName(sb, type.DeclaringType, false);
				sb.Append('+');
			}

			if (type.IsGenericType)
			{
			    var name = type.GetGenericTypeDefinition().Name;

                //remove `1
			    int index = name.IndexOf('`');
                if(index > 0)
			        name = name.Remove(index);
                //

			    sb.Append(name);
				sb.Append('[');

				Type[] arguments = type.GetGenericArguments();
				for (int i = 0; i < arguments.Length; i++)
				{
					if (i > 0)
						sb.Append(',');

					sb.Append('[');
					GetMessageName(sb, arguments[i], true);
					sb.Append(']');
				}

				sb.Append(']');
			}
			else
				sb.Append(type.Name);

			return sb.ToString();
		}
	}
}