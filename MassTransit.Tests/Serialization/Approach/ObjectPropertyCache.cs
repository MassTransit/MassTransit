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
namespace MassTransit.Tests.Serialization.Approach
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public interface IObjectPropertyCache
	{
	}

	public class ObjectPropertyCache :
		IEnumerable<ObjectProperty>,
		IObjectPropertyCache
	{
		private const BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.Instance;

		private readonly Dictionary<string, ObjectProperty> _properties;

		public ObjectPropertyCache(Type type)
		{
			_properties = new Dictionary<string, ObjectProperty>();

			var properties = GetAllPropertiesForType(type).Select(x => new ObjectProperty(x));
			foreach (ObjectProperty property in properties)
			{
				_properties.Add(property.Name, property);
			}
		}

		public IEnumerator<ObjectProperty> GetEnumerator()
		{
			return _properties.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool TryGetProperty(string name, out ObjectProperty property)
		{
			return _properties.TryGetValue(name, out property);
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
	}
}