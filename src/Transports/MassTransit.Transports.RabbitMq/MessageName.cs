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
namespace MassTransit.Transports.RabbitMq
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Text;

	/// <summary>
	/// Class encapsulating naming strategies for exchanges corresponding
	/// to message types.
	/// </summary>
	[Serializable]
	public class MessageName :
		ISerializable
	{
		[ThreadStatic]
		static IDictionary<Type, string> _nameCache;

		readonly string _name;

		public MessageName(Type messageType)
		{
			_name = GetNameForType(messageType);
		}

		protected MessageName(SerializationInfo info, StreamingContext context)
		{
			_name = info.GetString("Name");
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", _name);
		}

		public override string ToString()
		{
			return _name ?? "";
		}

		static string IsInCache(Type type, Func<Type, string> provider)
		{
			if (_nameCache == null)
				_nameCache = new Dictionary<Type, string>();

			string urn;
			if (_nameCache.TryGetValue(type, out urn))
				return urn;

			urn = provider(type);

			_nameCache[type] = urn;

			return urn;
		}

		static string GetNameForType(Type type)
		{
			return IsInCache(type, x =>
				{
					if (type.IsGenericTypeDefinition)
						throw new ArgumentException("An open generic type cannot be used as a message name");

					var sb = new StringBuilder("");

					return GetMessageName(sb, type, null);
				});
		}

		static string GetMessageName(StringBuilder sb, Type type, string scope)
		{
			if (type.IsGenericParameter)
				return "";

			if (type.Namespace != null)
			{
				string ns = type.Namespace;
				if (!ns.Equals(scope))
				{
					sb.Append(ns);
					sb.Append(':');
				}
			}

			if (type.IsNested)
			{
				GetMessageName(sb, type.DeclaringType, type.Namespace);
				sb.Append('-');
			}

			if (type.IsGenericType)
			{
				string name = type.GetGenericTypeDefinition().Name;

				//remove `1
				int index = name.IndexOf('`');
				if (index > 0)
					name = name.Remove(index);

				sb.Append(name);
				sb.Append("--");

				Type[] arguments = type.GetGenericArguments();
				for (int i = 0; i < arguments.Length; i++)
				{
					if (i > 0)
						sb.Append("::");

					GetMessageName(sb, arguments[i], type.Namespace);
				}

				sb.Append("--");
			}
			else
				sb.Append(type.Name);

			return sb.ToString();
		}
	}
}