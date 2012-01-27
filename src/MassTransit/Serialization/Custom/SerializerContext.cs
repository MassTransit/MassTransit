// Copyright 2007-2010 The Apache Software Foundation.
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Xml;
	using Logging;
	using Magnum.Reflection;
    using Magnum.Extensions;
	using TypeSerializers;

	public class SerializerContext :
		ISerializerContext
	{
		private static readonly ILog _log = Logger.Get(typeof (SerializerContext));

		private static readonly Dictionary<Type, IObjectSerializer> _serializers;

		private readonly NamespaceTable _namespaceTable = new NamespaceTable();
		private readonly SerializerTypeMapper _mapper;

		static SerializerContext()
		{
			_serializers = new Dictionary<Type, IObjectSerializer>();

			LoadBuiltInSerializers();
		}

		public SerializerContext()
		{
			_mapper = (d, p, o) => p == typeof (object) ? o.GetType() : p;
		}

		public SerializerContext(SerializerTypeMapper mapper)
		{
			_mapper = mapper;
		}

		public string GetPrefix(string localName, string ns)
		{
			return _namespaceTable.GetPrefix(localName, ns);
		}

		public Type MapType(Type declaringType, Type propertyType, object value)
		{
			return _mapper(declaringType, propertyType, value);
		}

		public void WriteNamespaceInformationToXml(XmlWriter writer)
		{
			_namespaceTable.Each((ns, prefix) => writer.WriteAttributeString("xmlns", prefix, null, ns));
		}

		public IEnumerable<Continuation<Action<XmlWriter>>> SerializeObject(string localName, Type type, object value)
		{
			IObjectSerializer serializer = GetSerializerFor(type);

			foreach (Continuation<Action<XmlWriter>> action in serializer.GetSerializationActions(this, localName, value))
			{
				yield return action;
			}
		}

		public IEnumerable<Continuation<Action<XmlWriter>>> Serialize<T>(T value)
			where T : class
		{
			if (value == null)
				yield break;

			Type type = typeof(T);
			string localName = type.ToXmlFriendlyName();

			foreach (Continuation<Action<XmlWriter>> action in SerializeObject(localName, type, value))
			{
				yield return action;
			}
		}

		private static IObjectSerializer GetSerializerFor(Type type)
		{
			IObjectSerializer serializer;
			lock(_serializers)
			{
				serializer = _serializers.Retrieve(type, () => CreateSerializerFor(type));
			}

			return serializer;
		}

		private static IObjectSerializer CreateSerializerFor(Type type)
		{
			if (type.IsEnum)
			{
				return (IObjectSerializer)FastActivator.Create(typeof(EnumSerializer<>).MakeGenericType(type));
			}

			if (typeof (IEnumerable).IsAssignableFrom(type))
			{
				return CreateEnumerableSerializerFor(type);
			}

			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				Type nullableType = type.GetGenericArguments().First();

				return _serializers.Retrieve(nullableType, () => CreateSerializerFor(nullableType));
			}

			Type serializerType = typeof (ObjectSerializer<>).MakeGenericType(type);

			var serializer = (IObjectSerializer)FastActivator.Create(serializerType);

			return serializer;
		}

		private static IObjectSerializer CreateEnumerableSerializerFor(Type type)
		{
			if(type.IsArray)
			{
				return (IObjectSerializer)FastActivator.Create(typeof(ArraySerializer<>).MakeGenericType(type.GetElementType()));				
			}

			Type[] genericArguments = type.GetDeclaredGenericArguments().ToArray();
			if (genericArguments.Length == 0)
			{
				Type elementType = type.IsArray ? type.GetElementType() : typeof (object);

				Type serializerType = typeof (ArraySerializer<>).MakeGenericType(elementType);

				return (IObjectSerializer)FastActivator.Create(serializerType);
			}

			if (type.ImplementsGeneric(typeof(IDictionary<,>)))
			{
				Type serializerType = typeof (DictionarySerializer<,>).MakeGenericType(genericArguments);

				return (IObjectSerializer)FastActivator.Create(serializerType);
			}

			if (type.ImplementsGeneric(typeof (IList<>)) || type.ImplementsGeneric(typeof (IEnumerable<>)))
			{
				Type serializerType = typeof (ListSerializer<>).MakeGenericType(genericArguments[0]);

				return (IObjectSerializer)FastActivator.Create(serializerType);
			}

			throw new InvalidOperationException("enumerations not yet supported");
		}

		private static void LoadBuiltInSerializers()
		{
			var query = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(x => x.BaseType != null && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof (SerializerBase<>));

			foreach (Type type in query)
			{
                if (type.BaseType != null)
                {
                    Type itemType = type.BaseType.GetGenericArguments()[0];

                    _log.DebugFormat("Adding serializer for {0} ({1})", itemType.Name, type.Name);

                    _serializers.Add(itemType, FastActivator.Create(type) as IObjectSerializer);
                }
			}
		}
	}

	public static class UsefulExtensionMethods
	{
		public static bool ImplementsGeneric(this Type type, Type targetType)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == targetType)
				return true;

			var count = type.GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == targetType)
				.Count();

			if (count > 0)
				return true;

			var baseType = type.BaseType;
			if (baseType == null)
				return false;

			return baseType.IsGenericType
			       	? baseType.GetGenericTypeDefinition() == targetType
			       	: baseType.ImplementsGeneric(targetType);
		}

		public static string ToXmlFriendlyName(this Type type)
		{
			string name = type.Name;

			if (type.IsGenericType)
			{
				int index = name.IndexOf('`');
				if (index > 0)
					return name.Substring(0, index);

			}

			return name;
		}
	}
}