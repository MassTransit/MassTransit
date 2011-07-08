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
namespace MassTransit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using RequestResponse;

	public static class ServiceBusExtensions
	{
		/// <summary>
		/// Make a request to a service and wait for the response to be received, built
		/// using a fluent interface with a final call to Send()
		/// </summary>
		/// <param name="bus"></param>
		/// <param name="requestAction"></param>
		/// <returns></returns>
		public static RequestResponseScope MakeRequest(this IServiceBus bus, Action<IServiceBus> requestAction)
		{
			return new RequestResponseScope(bus, requestAction);
		}

		public static ResponseActionBuilder<T> When<T>(this RequestResponseScope scope)
			where T : class
		{
			return new ResponseActionBuilder<T>(scope);
		}

		public static string ToMessageName(this Type messageType)
		{
			string messageName;
			if (messageType.IsGenericType)
			{
				messageName = messageType.GetGenericTypeDefinition().FullName;
				messageName += "[";
				string prefix = "";
				foreach (Type argument in messageType.GetGenericArguments())
				{
					messageName += prefix + "[" + argument.ToMessageName() + "]";
					prefix = ",";
				}
				messageName += "]";
			}
			else
			{
				messageName = messageType.FullName;
			}

			string assembly = messageType.Assembly.FullName;
			if (assembly != null)
			{
				assembly = ", " + assembly.Substring(0, assembly.IndexOf(','));
			}
			else
			{
				assembly = String.Empty;
			}

			return String.Format("{0}{1}", messageName, assembly);
		}

		/// <summary>
		/// Returns true if the specified type is an allowed message type
		/// </summary>
		/// <param name="type">The type to inspect</param>
		/// <returns>True if the message can be sent, otherwise false</returns>
		public static bool IsAllowedMessageType(this Type type)
		{
			if (type.Namespace == null)
				return false;

			if (type.Assembly == typeof (object).Assembly)
				return false;

			if (type.Namespace == "System")
				return false;

			if (type.Namespace.StartsWith("System."))
				return false;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (CorrelatedBy<>))
				return false;

			return true;
		}

		/// <summary>
		/// Returns all the message types that are available for the specified type. This will
		/// return any base classes or interfaces implemented by the type that are allowed
		/// message types.
		/// </summary>
		/// <param name="type">The type to inspect</param>
		/// <returns>An enumeration of valid message types implemented by the specified type</returns>
		public static IEnumerable<Type> GetMessageTypes(this Type type)
		{
			yield return type;

			Type baseType = type.BaseType;
			while ((baseType != null) && baseType.IsAllowedMessageType())
			{
				yield return baseType;

				baseType = baseType.BaseType;
			}

			IEnumerable<Type> interfaces = type
				.GetInterfaces()
				.Where(IsAllowedMessageType);

			foreach (Type interfaceType in interfaces)
			{
				yield return interfaceType;
			}
		}
	}
}