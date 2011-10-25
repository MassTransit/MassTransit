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
	using System.Xml;

	public class ObjectSerializer<T> :
		IObjectSerializer
	{
		private readonly string _ns;
		private readonly IEnumerable<SerializeObjectProperty<T>> _propertyCache;
		private readonly Type _type;

		public ObjectSerializer()
		{
			_propertyCache = new SerializeObjectPropertyCache<T>();

			_type = typeof (T);
			_ns = _type.AssemblyQualifiedName;//.ToMessageName();
		}

		public IEnumerable<Continuation<Action<XmlWriter>>> GetSerializationActions(ISerializerContext context, string localName, object value)
		{
			if (value == null)
				yield break;

			string prefix = context.GetPrefix(localName, _ns);

			yield return output => output(writer =>
				{
					bool isDocumentElement = writer.WriteState == WriteState.Start;

					writer.WriteStartElement(prefix, localName, _ns);

					if (isDocumentElement)
						context.WriteNamespaceInformationToXml(writer);
				});

			foreach (SerializeObjectProperty<T> property in _propertyCache)
			{
				object obj = property.GetValue((T) value);
				if (obj == null)
					continue;

				var serializeType = context.MapType(typeof (T), property.PropertyType, obj);
				IEnumerable<Continuation<Action<XmlWriter>>> enumerable = context.SerializeObject(property.Name, serializeType, obj);
				foreach (Continuation<Action<XmlWriter>> action in enumerable)
				{
					yield return action;
				}
			}

			yield return output => output(writer => { writer.WriteEndElement(); });
		}
	}
}