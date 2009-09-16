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
	using System.Collections.Generic;
	using System.Linq;

	public class DeserializeObjectPropertyCache<T> :
		IDeserializeObjectPropertyCache<T>
	{
		private readonly Dictionary<string, DeserializeObjectProperty<T>> _properties;

		public DeserializeObjectPropertyCache()
		{
			_properties = new Dictionary<string, DeserializeObjectProperty<T>>();

			var properties = typeof(T).GetAllProperties()
				.Where(x => x.GetGetMethod() != null)
				.Where(x => x.GetSetMethod(true) != null)
				.Select(x => new DeserializeObjectProperty<T>(x));

			foreach (DeserializeObjectProperty<T> property in properties)
			{
				_properties.Add(property.Name, property);
			}
		}

		public bool TryGetProperty(string name, out DeserializeObjectProperty<T> property)
		{
			return _properties.TryGetValue(name, out property);
		}
	}
}