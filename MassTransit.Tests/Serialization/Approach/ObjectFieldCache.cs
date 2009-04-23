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

	public interface IObjectFieldCache
	{
	}

	public class ObjectFieldCache :
		IEnumerable<ObjectField>,
		IObjectFieldCache
	{
		private const BindingFlags _bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

		private readonly List<ObjectField> _fields;

		public ObjectFieldCache(Type type)
		{
			_fields = new List<ObjectField>(GetAllFieldsForType(type));
		}

		public IEnumerator<ObjectField> GetEnumerator()
		{
			return _fields.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static IEnumerable<ObjectField> GetAllFieldsForType(Type type)
		{
			return type.GetFields(_bindingFlags).Select(x => new ObjectField(x));
		}
	}
}