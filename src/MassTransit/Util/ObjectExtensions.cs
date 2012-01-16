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
namespace MassTransit.Util
{
	using System;

	public static class ObjectExtensions
	{
		public static T TranslateTo<T>(this object input)
			where T : class
		{
			if (input == null)
				throw new ArgumentNullException("input");

			var result = input as T;
			if (result == null)
				throw new InvalidOperationException("Unable to convert from " + input.GetType().FullName + " to " +
				                                    typeof (T).FullName);

			return result;
		}

		public static string ToFriendlyName(this Type type)
		{
			if (!type.IsGenericType)
			{
				return type.FullName;
			}

			string name = type.GetGenericTypeDefinition().FullName;
            if (name == null)
                return type.Name;

			name = name.Substring(0, name.IndexOf('`'));
			name += "<";

			Type[] arguments = type.GetGenericArguments();
			for (int i = 0; i < arguments.Length; i++)
			{
				if (i > 0)
					name += ",";

				name += arguments[i].Name;
			}

			name += ">";

			return name;
		}
	}
}