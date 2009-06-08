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
	using System.Reflection;
	using Magnum.Reflection;

	public class ObjectProperty<T>
	{
		private FastProperty<T> _fastProperty;

		public ObjectProperty(PropertyInfo info)
		{
			_fastProperty = new FastProperty<T>(info, BindingFlags.NonPublic);
		}

		public string Name
		{
			get { return _fastProperty.Property.Name; }
		}

		public Type PropertyType
		{
			get { return _fastProperty.Property.PropertyType; }
		}

		public object GetValue(T instance)
		{
			return _fastProperty.Get(instance);
		}

		public void SetValue(T instance, object value)
		{
			_fastProperty.Set(instance, value);
		}
	}
}