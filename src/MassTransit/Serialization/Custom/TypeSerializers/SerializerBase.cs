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

	public abstract class SerializerBase<T> :
		IObjectSerializer
	{
		private static readonly string _namespace;
		private static readonly Type _type;

		static SerializerBase()
		{
			_type = typeof (T);
			_namespace = _type.AssemblyQualifiedName;//.ToMessageName();
		}

		public IEnumerable<Continuation<Action<XmlWriter>>> GetSerializationActions(ISerializerContext context, string localName, object value)
		{
			string prefix = context.GetPrefix(_type.Name, _namespace);

			yield return output => output(writer =>
				{
					bool isDocumentElement = writer.WriteState == WriteState.Start;

					writer.WriteStartElement(prefix, localName, _namespace);

					if (isDocumentElement)
						context.WriteNamespaceInformationToXml(writer);

					if(value != null)
						WriteValue(writer, value);
                    
					writer.WriteEndElement();
				});
		}

		protected abstract void WriteValue(XmlWriter writer, object value);
	}
}