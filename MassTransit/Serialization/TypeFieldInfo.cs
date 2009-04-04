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

	public class TypeFieldInfo : 
		IEnumerable<FieldInfo>
	{
		private const BindingFlags _bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

		private readonly List<FieldInfo> _fields;

		public TypeFieldInfo(Type type)
		{
			_fields = new List<FieldInfo>(GetAllFieldsForType(type));
		}

		private static IEnumerable<FieldInfo> GetAllFieldsForType(Type type)
		{
			return type.GetFields(_bindingFlags);
		}

		public IEnumerator<FieldInfo> GetEnumerator()
		{
			return _fields.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}