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
	using System.Linq.Expressions;
	using System.Reflection;

	public class DeserializeObjectProperty<T> :
		ObjectPropertyBase
	{
		private readonly Action<T, object> _set;

		public DeserializeObjectProperty(PropertyInfo info)
			: base(info)
		{
			_set = InitializeSet(info);
		}

		public void SetValue(T instance, object value)
		{
			_set(instance, value);
		}

		private static Action<T, object> InitializeSet(PropertyInfo property)
		{
			var instance = Expression.Parameter(typeof (T), "instance");
			var value = Expression.Parameter(typeof (object), "value");

			UnaryExpression valueCast;
			if (property.PropertyType.IsValueType)
				valueCast = Expression.Convert(value, property.PropertyType);
			else
				valueCast = Expression.TypeAs(value, property.PropertyType);

			var call = Expression.Call(instance, property.GetSetMethod(true), valueCast);

			return Expression.Lambda<Action<T, object>>(call, new[] {instance, value}).Compile();
		}
	}
}