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
namespace MassTransit.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Xml;
	using Internal;
	using log4net;
	using Magnum.Reflection;

	public class CustomXmlMessageSerializer : 
		IMessageSerializer
	{
		private static readonly string BASETYPE = "baseType";
		private static readonly ILog _log = LogManager.GetLogger(typeof(CustomXmlMessageSerializer));

		private static readonly List<Type> typesBeingInitialized = new List<Type>();
		private static readonly Dictionary<Type, Type> typesToCreateForArrays = new Dictionary<Type, Type>();
		private static readonly Dictionary<Type, IEnumerable<FieldInfo>> typeToFields = new Dictionary<Type, IEnumerable<FieldInfo>>();
		private static readonly Dictionary<Type, IEnumerable<PropertyInfo>> typeToProperties = new Dictionary<Type, IEnumerable<PropertyInfo>>();
		private static readonly string XMLPREFIX = "d1p1";
		private static readonly string XMLTYPE = XMLPREFIX + ":type";

		[ThreadStatic]
		private static string _defaultNamespace;

		[ThreadStatic]
		private static List<Type> _messageBaseTypes;

		/// <summary>
		/// Used for serialization
		/// </summary>
		[ThreadStatic]
		private static IDictionary<string, string> _namespacesToPrefix;

		/// <summary>
		/// Used for deserialization
		/// </summary>
		[ThreadStatic]
		private static IDictionary<string, string> _prefixesToNamespaces;

		public CustomXmlMessageSerializer()
			: this(new MessageMapper())
		{

			
		}

		public CustomXmlMessageSerializer(IMessageMapper messageMapper)
		{
			Namespace = "http://tempuri.org/";

			_messageMapper = messageMapper;
		}

		private IMessageMapper _messageMapper;

		/// <summary>
		/// The namespace to place in outgoing XML.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// Gets/sets additional types to be serialized on top of those detected by the caller of Initialize.
		/// </summary>
		public List<Type> AdditionalTypes { get; set; }

		/// <summary>
		/// Deserializes the given stream to an array of messages which are returned.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public object Deserialize(Stream stream)
		{
			_prefixesToNamespaces = new Dictionary<string, string>();
			_messageBaseTypes = new List<Type>();

			XmlDocument doc = new XmlDocument();
			doc.Load(stream);

			XmlElement documentElement = doc.DocumentElement;
			if (documentElement == null)
				throw new InvalidOperationException("Unable to parse to XML document element");

			foreach (XmlAttribute attr in documentElement.Attributes)
			{
				if (attr.Name == "xmlns")
					_defaultNamespace = attr.Value.Substring(attr.Value.LastIndexOf("/") + 1);
				else
				{
					if (attr.Name.Contains("xmlns:"))
					{
						int colonIndex = attr.Name.LastIndexOf(":");
						string prefix = attr.Name.Substring(colonIndex + 1);

						if (prefix.Contains(BASETYPE))
						{
							Type baseType = _messageMapper.GetMappedTypeFor(attr.Value);
							if (baseType != null)
								_messageBaseTypes.Add(baseType);
						}
						else
							_prefixesToNamespaces[prefix] = attr.Value;
					}
				}
			}

			object message = Process(documentElement, null);
			if(message == null)
					throw new SerializationException("Could not deserialize message.");

			if(message is XmlMessageEnvelope)
			{
				XmlMessageEnvelope envelope = message as XmlMessageEnvelope;
				// process out headers

				InboundMessageHeaders.SetCurrent(envelope.GetMessageHeadersSetAction());

				return envelope.Message;
			}

			return message;
		}

		/// <summary>
		/// Serializes the given messages to the given stream.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="stream"></param>
		public void Serialize<T>(Stream stream, T message)
		{
			var envelope = XmlMessageEnvelope.Create(message);



			//_namespacesToPrefix = new Dictionary<string, string>();

			var settings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true};

			using (var streamWriter = new StreamWriter(stream))
			using (var writer = XmlWriter.Create(streamWriter, settings))
			{
			//	List<string> namespaces = GetNamespaces(message, _messageMapper);
			//	List<string> baseTypes = GetBaseTypes(message, _messageMapper);


				XmlObjectSerializer serializer = XmlObjectSerializer.GetSerializerForType(typeof (XmlMessageEnvelope));

				serializer.WriteObject(writer, null, "envelope", envelope, x =>
					{
						x.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
						x.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
					});

//				for (int i = 0; i < namespaces.Count; i++)
//				{
//					string prefix = "q" + i;
//					if (i == 0)
//						prefix = "";
//
//					writer.WriteAttributeString("xmlns" + (prefix != "" ? ":" + prefix : prefix), Namespace + "/" + namespaces[i]);
//
////					builder.AppendFormat(" xmlns{0}=\"{1}/{2}\"", (prefix != "" ? ":" + prefix : prefix), Namespace, namespaces[i]);
//					_namespacesToPrefix[namespaces[i]] = prefix;
//				}
//
//				for (int i = 0; i < baseTypes.Count; i++)
//				{
//					string prefix = BASETYPE;
//					if (i != 0)
//						prefix += i;
//
//					writer.WriteAttributeString("xmlns", prefix, null, baseTypes[i]);
//
//					//builder.AppendFormat(" xmlns:{0}=\"{1}\"", prefix, baseTypes[i]);
//				}

				writer.WriteEndDocument();

				writer.Flush();
				streamWriter.Flush();
			}
		}

//		/// <summary>
//		/// Initializes the serializer, passing the given types in addition to those in AdditionalTypes to the message mapper.
//		/// </summary>
//		/// <param name="types"></param>
//		public void Initialize(params Type[] types)
//		{
//			if (AdditionalTypes == null)
//				AdditionalTypes = new List<Type>();
//
//			AdditionalTypes.Add(typeof (XmlMessageEnvelope));
//			AdditionalTypes.AddRange(types);
//			_messageMapper.Initialize(AdditionalTypes.ToArray());
//		}

//		public void InitType(Type t)
//		{
//			if (t.IsPrimitive || t == typeof (string) || t == typeof (Guid) || t == typeof (DateTime))
//				return;
//
//			if (typeof (IEnumerable).IsAssignableFrom(t))
//			{
//				if (t.IsArray)
//					typesToCreateForArrays[t] = typeof (List<>).MakeGenericType(t.GetElementType());
//
//				foreach (Type g in t.GetGenericArguments())
//					InitType(g);
//
//				return;
//			}
//
//			//already in the process of initializing this type (prevents infinite recursion).
//			if (typesBeingInitialized.Contains(t))
//				return;
//
//			typesBeingInitialized.Add(t);
//
//			var props = GetAllPropertiesForType(t);
//			typeToProperties[t] = props;
//			var fields = GetAllFieldsForType(t);
//			typeToFields[t] = fields;
//
//			foreach (PropertyInfo prop in props)
//				InitType(prop.PropertyType);
//
//			foreach (FieldInfo field in fields)
//				InitType(field.FieldType);
//		}

		private object Process(XmlNode node, object parent)
		{
			XmlAttribute typeAttribute = node.Attributes["type", "http://www.w3.org/2001/XMLSchema-instance"];
			if (typeAttribute == null)
				throw new SerializationException("Unable to deserialize message body, no type information found");

			string typeName = typeAttribute.Value;







			string name = node.Name;

			if (name.Contains(":"))
			{
				int colonIndex = node.Name.IndexOf(":");
				name = name.Substring(colonIndex + 1);
				string prefix = node.Name.Substring(0, colonIndex);
				string nameSpace = _prefixesToNamespaces[prefix];

				typeName = nameSpace.Substring(nameSpace.LastIndexOf("/") + 1) + "." + name;
			}

			if (parent != null)
			{
				if (parent is IEnumerable)
				{
					if (parent.GetType().IsArray)
						return XmlObjectSerializer.GetSerializerForType(parent.GetType().GetElementType()).ReadObject(node);

					var args = parent.GetType().GetGenericArguments();
					if (args.Length == 1)
						return XmlObjectSerializer.GetSerializerForType(args[0]).ReadObject(node);
				}

				PropertyInfo prop = parent.GetType().GetProperty(name);
				if (prop != null)
					return XmlObjectSerializer.GetSerializerForType(prop.PropertyType).ReadObject(node);
			}

			Type t = _messageMapper.GetMappedTypeFor(typeName);
			if (t == null)
			{
				_log.Debug("Could not load " + typeName + ". Trying base types...");
				foreach (Type baseType in _messageBaseTypes)
					try
					{
						_log.Debug("Trying to deserialize message to " + baseType.FullName);
						return XmlObjectSerializer.GetSerializerForType(baseType).ReadObject(node);
					}
					catch
					{
					} // intentionally swallow exception

				throw new TypeLoadException("Could not handle type '" + typeName + "'.");
			}

			return XmlObjectSerializer.GetSerializerForType(t).ReadObject(node);
		}

		private object GetObjectOfTypeFromNode(Type t, XmlNode node)
		{
			if (t.IsSimpleType())
				return GetPropertyValue(t, node, null);

			object result = ClassFactory.New(t);

			foreach (XmlNode n in node.ChildNodes)
			{
				PropertyInfo prop = GetProperty(t, n.Name);
				if (prop != null)
				{
					object val = GetPropertyValue(prop.PropertyType, n, result);
					if (val != null)
						prop.SetValue(result, val, null);
				}

				FieldInfo field = GetField(t, n.Name);
				if (field != null)
				{
					object val = GetPropertyValue(field.FieldType, n, result);
					if (val != null)
						field.SetValue(result, val);
				}
			}

			return result;
		}

		private PropertyInfo GetProperty(Type t, string name)
		{
			IEnumerable<PropertyInfo> props;
			typeToProperties.TryGetValue(t, out props);

			if (props == null)
				return null;

			foreach (PropertyInfo prop in props)
				if (prop.Name == name)
					return prop;

			return null;
		}

		private FieldInfo GetField(Type t, string name)
		{
			IEnumerable<FieldInfo> fields;
			typeToFields.TryGetValue(t, out fields);

			if (fields == null)
				return null;

			foreach (FieldInfo f in fields)
				if (f.Name == name)
					return f;

			return null;
		}

		private object GetPropertyValue(Type type, XmlNode n, object parent)
		{
			if (n.ChildNodes.Count == 1 && n.ChildNodes[0] is XmlText)
			{
				if (type == typeof (string))
					return n.ChildNodes[0].InnerText;

				if (type.IsPrimitive || type == typeof (decimal))
					return Convert.ChangeType(n.ChildNodes[0].InnerText, type);

				if (type == typeof (Guid))
					return new Guid(n.ChildNodes[0].InnerText);

				if (type == typeof (DateTime))
					return XmlConvert.ToDateTime(n.ChildNodes[0].InnerText, XmlDateTimeSerializationMode.Utc);

				if (type == typeof (TimeSpan))
					return XmlConvert.ToTimeSpan(n.ChildNodes[0].InnerText);

				if (type.IsEnum)
					return Enum.Parse(type, n.ChildNodes[0].InnerText);
			}

			if (typeof (IEnumerable).IsAssignableFrom(type) && type != typeof (string))
			{
				bool isArray = type.IsArray;

				Type typeToCreate = type;
				if (isArray)
					typeToCreate = typesToCreateForArrays[type];

				IList list = Activator.CreateInstance(typeToCreate) as IList;

				foreach (XmlNode xn in n.ChildNodes)
				{
					object m = Process(xn, list);

					if (list != null)
						list.Add(m);
				}

				if (isArray)
					return typeToCreate.GetMethod("ToArray").Invoke(list, null);

				return list;
			}

			if (n.ChildNodes.Count == 0)
				if (type == typeof (string))
					return string.Empty;
				else
					return null;


			return GetObjectOfTypeFromNode(type, n);
		}

//		private void Write(XmlWriter writer, Type t, object obj)
//		{
//			if (obj == null)
//				return;
//
//			foreach (PropertyInfo prop in typeToProperties[t])
//				WriteEntry(prop.Name, prop.PropertyType, prop.GetValue(obj, null), writer);
//
//			foreach (FieldInfo field in typeToFields[t])
//				WriteEntry(field.Name, field.FieldType, field.GetValue(obj), writer);
//		}

//		private void WriteObject(string name, Type type, object value, XmlWriter writer)
//		{
//			string element = name;
//			string prefix;
//			_namespacesToPrefix.TryGetValue(type.Namespace, out prefix);
//
//			if (!string.IsNullOrEmpty(prefix))
//				element = prefix + ":" + name;
//
//
//			writer.WriteStartElement(element);
////			builder.AppendFormat("<{0}>\n", element);
//
//			Write(writer, type, value);
//
////			builder.AppendFormat("</{0}>\n", element);
//			writer.WriteEndElement();
//		}

//		private void WriteEntry(string name, Type type, object value, XmlWriter writer)
//		{
//			if (value == null)
//				return;
//
//			if (type.IsValueType || type == typeof (string))
//			{
//				//writer.AppendFormat("<{0}>{1}</{0}>\n", name, FormatAsString(value));
//				writer.WriteElementString(name, FormatAsString(value));
//				return;
//			}
//
//			if (typeof (IEnumerable).IsAssignableFrom(type))
//			{
//				//writer.AppendFormat("<{0}>\n", name);
//				writer.WriteStartElement(name);
//
//				Type baseType = typeof (object);
//				Type[] generics = type.GetGenericArguments();
//				if (generics != null && generics.Length > 0)
//					baseType = generics[0];
//
//				foreach (object obj in ((IEnumerable) value))
//					if (obj.GetType().IsSimpleType())
//						WriteEntry(obj.GetType().Name, obj.GetType(), obj, writer);
//					else
//						WriteObject(baseType.Name, baseType, obj, writer);
//
//				//writer.AppendFormat("</{0}>\n", name);
//				writer.WriteEndElement();
//				return;
//			}
//
//			WriteObject(name, type, value, writer);
//		}

//		private string FormatAsString(object value)
//		{
//			if (value == null)
//				return string.Empty;
//			if (value is bool)
//				return value.ToString().ToLower();
//			if (value is string)
//				return value as string;// SecurityElement.Escape(value as string);
//			if (value is DateTime)
//				return ((DateTime) value).ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
//			if (value is TimeSpan)
//			{
//				TimeSpan ts = (TimeSpan) value;
//				return string.Format("{0}P0Y0M{1}DT{2}H{3}M{4}.{5:000}S", (ts.TotalSeconds < 0 ? "-" : ""), Math.Abs(ts.Days), Math.Abs(ts.Hours), Math.Abs(ts.Minutes), Math.Abs(ts.Seconds), Math.Abs(ts.Milliseconds));
//			}
//			if (value is Guid)
//				return ((Guid) value).ToString();
//
//			return value.ToString();
//		}

		private static List<string> GetNamespaces(object message, IMessageMapper mapper)
		{
			List<string> result = new List<string>();

			string ns = mapper.GetMappedTypeFor(message.GetType()).Namespace;
			if (!result.Contains(ns))
				result.Add(ns);

			return result;
		}

		private static List<string> GetBaseTypes(object message, IMessageMapper mapper)
		{
			List<string> result = new List<string>();

			Type t = mapper.GetMappedTypeFor(message.GetType());

			Type baseType = t.BaseType;
			while (baseType != typeof (object) && baseType != null)
			{
				if (typeof (object).IsAssignableFrom(baseType))
					if (!result.Contains(baseType.FullName))
						result.Add(baseType.FullName);

				baseType = baseType.BaseType;
			}

			foreach (Type i in t.GetInterfaces())
				if (i != typeof (object) && typeof (object).IsAssignableFrom(i))
					if (!result.Contains(i.FullName))
						result.Add(i.FullName);

			return result;
		}
	}


	/// <summary>
	/// Contains extension methods
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Returns true if the type can be serialized as is.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsSimpleType(this Type type)
		{
			return (type.IsPrimitive ||
			        type == typeof (string) ||
			        type == typeof (decimal) ||
			        type == typeof (Guid) ||
			        type == typeof (DateTime) ||
			        type == typeof (TimeSpan) ||
			        type.IsEnum);
		}
	}
}