/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus
{
	using System;
	using System.Collections.Generic;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple =  true, Inherited = false)]
	public class AllowMessageType : Attribute
	{
		private readonly Dictionary<Type, MessageTypeUsage> _types = new Dictionary<Type, MessageTypeUsage>();

		public AllowMessageType()
		{
		}

		public AllowMessageType(params Type[] types)
		{
			foreach (Type type in types)
			{
				_types[type] = MessageTypeUsage.Single;
			}
		}

		public MessageTypeUsage Usage
		{
			set
			{
				foreach (KeyValuePair<Type, MessageTypeUsage> type in _types)
				{
					_types[type.Key] = value;
				}
			}
		}

		public MessageTypeUsage GetUsage(Type t)
		{
			if (_types.ContainsKey(t) == false)
				return MessageTypeUsage.None;

			return _types[t];
		}
	}

	public enum MessageTypeUsage
	{
		None,
		Single,
		Multiple,
	}
}