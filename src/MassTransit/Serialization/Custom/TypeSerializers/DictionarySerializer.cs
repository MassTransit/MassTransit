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
namespace MassTransit.Serialization.Custom.TypeSerializers
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

	public class DictionarySerializer<TKey, TValue> :
		IObjectSerializer
	{
		private readonly Type _keyType;
		private readonly string _ns;
		private readonly Type _valueType;
		private readonly Type _containerType;

		public DictionarySerializer()
		{
			_valueType = typeof (TValue);
			_keyType = typeof (TKey);
			_containerType = typeof (Dictionary<TKey, TValue>);
			_ns = _containerType.AssemblyQualifiedName;//.ToMessageName();
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

			foreach (KeyValuePair<TKey, TValue> entry in ((IDictionary<TKey, TValue>) value))
			{
				yield return output => output(writer => writer.WriteStartElement("item"));

				// TODO would really like a way to write the key as an attribute string if it is a simple type
				var enumerable = context.SerializeObject("key", _keyType, entry.Key);
				foreach (var action in enumerable)
				{
					yield return action;
				}

				enumerable = context.SerializeObject(_valueType.Name, _valueType, entry.Value);
				foreach (var action in enumerable)
				{
					yield return action;
				}

				yield return output => output(writer => writer.WriteEndElement());
			}

			yield return output => output(writer => { writer.WriteEndElement(); });
		}
	}
}