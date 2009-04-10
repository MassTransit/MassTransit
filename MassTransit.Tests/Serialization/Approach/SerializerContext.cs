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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Xml;
	using log4net;
	using Magnum.Monads;
	using Magnum.Reflection;

	public class SerializerContext :
		ISerializerContext
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SerializerContext));

		private static readonly Dictionary<Type, IObjectSerializer> _serializers;

		private NamespaceTable _namespaceTable = new NamespaceTable();

		static SerializerContext()
		{
			_serializers = new Dictionary<Type, IObjectSerializer>
				{
					{typeof (object), new ObjectSerializer()},
				};

			var query = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(x => x.BaseType != null && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof (SerializerBase<>));

			foreach (Type type in query)
			{
				Type itemType = type.BaseType.GetGenericArguments()[0];

				_log.DebugFormat("Adding serializer for {0} ({1})", itemType.Name, type.Name);

				_serializers.Add(itemType, ClassFactory.New(type) as IObjectSerializer);
			}
		}

		public string GetPrefix(string localName, string ns)
		{
			return _namespaceTable.GetPrefix(localName, ns);
		}

		public void WriteNamespaceInformationToXml(XmlWriter writer)
		{
			_namespaceTable.Each((ns, prefix) => writer.WriteAttributeString("xmlns", prefix, null, ns));
		}


		public IEnumerable<K<Action<XmlWriter>>> SerializeObject(string localName, Type type, object value)
		{
			IObjectSerializer serializer;
			if (!_serializers.TryGetValue(type, out serializer))
			{
				serializer = _serializers[typeof (object)];
			}

			foreach (var action in serializer.GetSerializationActions(this, localName, value))
			{
				yield return action;
			}
		}

		public IEnumerable<K<Action<XmlWriter>>> Serialize(object value)
		{
			if (value == null)
				yield break;

			Type type = value.GetType();
			string localName = type.Name;

			foreach (var action in SerializeObject(localName, type, value))
			{
				yield return action;
			}
		}
	}
}