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
	using System.Runtime.Serialization;
	using System.Xml;
	using Logging;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using TypeDeserializers;

	public class DeserializerContext :
		IDeserializerContext
	{
		private readonly XmlReader _reader;
		private static readonly Dictionary<string, IObjectDeserializer> _deserializers;
		private static readonly Dictionary<Type, IObjectDeserializer> _deserializersByType;
		private static readonly ILog _log = Logger.Get(typeof (DeserializerContext));

		static DeserializerContext()
		{
			_deserializers = new Dictionary<string, IObjectDeserializer>();
			_deserializersByType = new Dictionary<Type, IObjectDeserializer>();

			LoadBuiltInDeserializers();
		}

		public DeserializerContext(XmlReader reader)
		{
			_reader = reader;
		}

		private static void LoadBuiltInDeserializers()
		{
			Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(x => !x.IsGenericType)
				.Where(x => x.Namespace == typeof(StringDeserializer).Namespace)
				.Each(type =>
					{
						type.GetInterfaces()
							.Where(x => x.IsGenericType &&  x.GetGenericTypeDefinition() == typeof (IObjectDeserializer<>))
							.Each(interfaceType =>
								{
									Type itemType = interfaceType.GetGenericArguments().First();

									_log.DebugFormat("Adding deserializer for {0} ({1})", itemType.Name, type.Name);

									var deserializer = FastActivator.Create(type) as IObjectDeserializer;

									_deserializers.Add(itemType.AssemblyQualifiedName, deserializer);
									_deserializersByType.Add(itemType, deserializer);
								});
					});
		}


		public object Deserialize()
		{
			while (_reader.Read())
			{
				if (_reader.NodeType == XmlNodeType.XmlDeclaration)
					continue;

				if (_reader.NodeType == XmlNodeType.Element)
				{
					return Deserialize(_reader.NamespaceURI);
				}

				if(_log.IsDebugEnabled)
					_log.DebugFormat("Unknown XML node: " + _reader.NodeType + " - " + (_reader.Name ?? "(none)") + " : " + _reader.NamespaceURI);
			}

			return null;
		}

		public object Deserialize(string ns)
		{
			IObjectDeserializer deserializer;
			lock (_deserializers)
			{
				deserializer = _deserializers.Retrieve(ns, () => CreateDeserializerFor(ns));
			}

			return deserializer.Deserialize(this);
		}

		public bool Read()
		{
			return _reader.Read();
		}

		public XmlNodeType NodeType
		{
			get { return _reader.NodeType; }
		}

		public string LocalName
		{
			get { return _reader.LocalName; }
		}

		public string Namespace
		{
			get { return _reader.NamespaceURI; }
		}

		public string Name
		{
			get { return _reader.Name; }
		}

		public bool IsEmptyElement
		{
			get { return _reader.IsEmptyElement; }
		}

		public void ExitElement()
		{
			if(NodeType == XmlNodeType.EndElement)
			{
				Read();
			}
		}

		public string ReadElementAsString()
		{
			return _reader.ReadElementContentAsString();
		}

		private static IObjectDeserializer CreateDeserializerFor(string ns)
		{
			Type type = Type.GetType(ns, false);
			if (type == null)
				type = GetTypeWithoutVersionString(ns);

			if (type == null)
				throw new SerializationException("Unable to deserialize an unknown type: " + ns);

			IObjectDeserializer deserializer;
			if(_deserializersByType.TryGetValue(type, out deserializer))
				return deserializer;

			deserializer = CreateDeserializerFor(type);

			if(!_deserializersByType.ContainsKey(type))
				_deserializersByType.Add(type, deserializer);

			return deserializer;
		}

		private static Type GetTypeWithoutVersionString(string ns)
		{
			int assemblyIndex = ns.IndexOf(",");
			if (assemblyIndex <= 0)
				return null;

			var assemblyName = new AssemblyName(ns.Substring(assemblyIndex + 1));

			var assembly = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.GetName().Name == assemblyName.Name
				            && x.GetName().GetPublicKeyToken().SequenceEqual(assemblyName.GetPublicKeyToken()))
				.FirstOrDefault();

			if(assembly == null)
				return null;

			string typeName = ns.Substring(0, assemblyIndex);

			return assembly.GetType(typeName, false);
		}

		private static IObjectDeserializer CreateDeserializerFor(Type type)
		{
			if (type.IsEnum)
			{
				return (IObjectDeserializer)FastActivator.Create(typeof(EnumDeserializer<>).MakeGenericType(type));
			}

			if (typeof (IEnumerable).IsAssignableFrom(type) && type != typeof (string))
			{
				if (type.IsArray)
				{
					return (IObjectDeserializer)FastActivator.Create(typeof(ArrayDeserializer<>).MakeGenericType(type.GetElementType()));
				}
				if (type.IsGenericType)
				{
					Type genericTypeDefinition = type.GetGenericTypeDefinition();
					Type[] arguments = type.GetGenericArguments();
					if (genericTypeDefinition == typeof (IList<>) || genericTypeDefinition == typeof (List<>))
					{
						return (IObjectDeserializer)FastActivator.Create(typeof(ListDeserializer<>).MakeGenericType(arguments));
					}

					if (genericTypeDefinition == typeof (IDictionary<,>) || genericTypeDefinition == typeof (Dictionary<,>))
					{
						return (IObjectDeserializer)FastActivator.Create(typeof(DictionaryDeserializer<,>).MakeGenericType(arguments));
					}
				}

				throw new NotSupportedException("Unsupported enumeration type: " + type.FullName);
			}

			Type deserializerType;
			if(type.IsInterface)
			{
				var proxyType = InterfaceImplementationBuilder.GetProxyFor(type);
				deserializerType = typeof (ObjectDeserializer<>).MakeGenericType(proxyType);
			}
			else
			{
				deserializerType = typeof(ObjectDeserializer<>).MakeGenericType(type);
			}

			return (IObjectDeserializer)FastActivator.Create(deserializerType);
		}
	}
}