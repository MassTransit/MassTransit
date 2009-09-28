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
namespace MassTransit.Serialization.Custom
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public static class ExtensionsForType
	{
		public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
		{
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

			foreach (PropertyInfo propertyInfo in type.GetProperties(bindingFlags))
			{
				yield return propertyInfo;
			}

			if (type.IsInterface)
			{
				foreach (Type interfaceType in type.GetInterfaces())
				{
					foreach (PropertyInfo propertyInfo in interfaceType.GetAllProperties())
					{
						yield return propertyInfo;
					}
				}
			}
		}
	}
}