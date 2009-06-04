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
	using Magnum.Monads;

	public class EnumerableSerializerBase<T> :
		IObjectSerializer
	{
		private Type _containerType;
		private string _ns;
		private Type _type;

		protected EnumerableSerializerBase(Type containerType)
		{
			_containerType = containerType;
			_type = typeof (T);
			_ns = _containerType.AssemblyQualifiedName;//.ToMessageName();
		}

		public IEnumerable<K<Action<XmlWriter>>> GetSerializationActions(ISerializerContext context, string localName, object value)
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

			foreach (var action in SerializeElements(value, context))
			{
				yield return action;
			}

			yield return output => output(writer => { writer.WriteEndElement(); });
		}

		protected virtual IEnumerable<K<Action<XmlWriter>>> SerializeElements(object value, ISerializerContext context)
		{
			foreach (T obj in ((IEnumerable<T>) value))
			{
				var enumerable = context.SerializeObject(obj.GetType().Name, obj.GetType(), obj);
				foreach (var action in enumerable)
				{
					yield return action;
				}
			}
		}
	}
}