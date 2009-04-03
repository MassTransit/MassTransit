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
namespace MassTransit.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	public class TypePropertyInfo : IEnumerable<PropertyInfo>
	{
		private const BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.Instance;

		private readonly List<PropertyInfo> _properties;

		public TypePropertyInfo(Type type)
		{
			_properties = new List<PropertyInfo>(GetAllPropertiesForType(type));
		}

		private static IEnumerable<PropertyInfo> GetAllPropertiesForType(Type type)
		{
			foreach (PropertyInfo propertyInfo in type.GetProperties(_bindingFlags))
			{
				yield return propertyInfo;
			}

			if (type.IsInterface)
			{
				foreach (Type interfaceType in type.GetInterfaces())
				{
					foreach (PropertyInfo propertyInfo in GetAllPropertiesForType(interfaceType))
					{
						yield return propertyInfo;
					}
				}
			}
		}

		public IEnumerator<PropertyInfo> GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}