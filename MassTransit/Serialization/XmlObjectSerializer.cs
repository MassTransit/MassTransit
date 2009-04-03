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
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml;
	using Magnum.CollectionExtensions;
	using Magnum.ObjectExtensions;
	using Magnum.Reflection;
	using Magnum.Threading;

	public class XmlObjectSerializer
	{
		private static readonly ReaderWriterLockedDictionary<Type, TypeFieldInfo> _fieldsForType;
		private static readonly ReaderWriterLockedDictionary<Type, TypePropertyInfo> _propertiesForType;

		private readonly string _baseNamespace = "http://tempuri.org/";
		private readonly TypeFieldInfo _fields;
		private readonly TypePropertyInfo _properties;
		private readonly Type _type;
		private static Dictionary<Type, Func<object, string>> _stringConverters;

		static XmlObjectSerializer()
		{
			_fieldsForType = new ReaderWriterLockedDictionary<Type, TypeFieldInfo>();
			_propertiesForType = new ReaderWriterLockedDictionary<Type, TypePropertyInfo>();

			_stringConverters = new Dictionary<Type, Func<object, string>>
				{
					{typeof (bool), x => XmlConvert.ToString((bool) x)},
					{typeof (byte), x => XmlConvert.ToString((byte) x)},
					{typeof (char), x => XmlConvert.ToString((char) x)},
					{typeof (DateTime), x => XmlConvert.ToString((DateTime) x, XmlDateTimeSerializationMode.Utc)},
					{typeof (DateTimeOffset), x => XmlConvert.ToString((DateTimeOffset) x)},
					{typeof (decimal), x => XmlConvert.ToString((decimal) x)},
					{typeof (double), x => XmlConvert.ToString((double) x)},
					{typeof (float), x => XmlConvert.ToString((float) x)},
					{typeof (Guid), x => XmlConvert.ToString((Guid) x)},
					{typeof (int), x => XmlConvert.ToString((int) x)},
					{typeof (long), x => XmlConvert.ToString((long) x)},
					{typeof (sbyte), x => XmlConvert.ToString((sbyte) x)},
					{typeof (short), x => XmlConvert.ToString((short) x)},
					{typeof (TimeSpan), x => XmlConvert.ToString((TimeSpan) x)},
					{typeof (uint), x => XmlConvert.ToString((uint) x)},
					{typeof (ulong), x => XmlConvert.ToString((ulong) x)},
					{typeof (ushort), x => XmlConvert.ToString((ushort) x)},
					{typeof(string),x => (string)x}
				};
		}

		private XmlObjectSerializer(Type type, TypePropertyInfo properties, TypeFieldInfo fields)
		{
			_type = type;
			_properties = properties;
			_fields = fields;
		}

		public void WriteObject(XmlWriter writer, object value)
		{
			string localName = _type.Name;

			WriteObject(writer, string.Empty, localName, value, x => { });
		}

		public void WriteObject(XmlWriter writer, string prefix, string localName, object value, Action<XmlWriter> elementAction)
		{
			if (prefix.IsNullOrEmpty())
				writer.WriteStartElement(localName);
			else
				writer.WriteStartElement(prefix, localName, _type.ToMessageName());

			elementAction(writer);

			writer.WriteAttributeString("xsi", "type", null, _type.ToMessageName());

			Write(writer, value);

			writer.WriteEndElement();
		}

		private void Write(XmlWriter writer, object value)
		{
			if (value == null)
				return;

			foreach (PropertyInfo prop in _properties)
				WriteEntry(prop.Name, prop.PropertyType, prop.GetValue(value, null), writer);

			foreach (FieldInfo field in _fields)
				WriteEntry(field.Name, field.FieldType, field.GetValue(value), writer);
		}

		private void WriteEntry(string name, Type type, object value, XmlWriter writer)
		{
			if (value == null)
				return;

			Func<object, string> converter;
			if(_stringConverters.TryGetValue(type, out converter))
			{
				writer.WriteElementString(name, converter(value));
				return;
			}
			
			if (typeof (IEnumerable).IsAssignableFrom(type))
			{
				WriteEnumerableEntry(value, type, writer, name);
				return;
			}

			var objectWriter = GetSerializerForType(value.GetType());

			objectWriter.WriteObject(writer, string.Empty, name, value, x => { });
		}

		private void WriteEnumerableEntry(object value, Type type, XmlWriter writer, string name)
		{
			writer.WriteStartElement(name);

			Type elementType = typeof(object);
			Type[] genericArguments = type.GetDeclaredGenericArguments().ToArray();
			if (genericArguments != null && genericArguments.Length > 0)
				elementType = genericArguments[0];

			writer.WriteAttributeString("xsi", "type", null, type.ToMessageName());

			foreach (object obj in ((IEnumerable) value))
				if (obj.GetType().IsSimpleType())
					WriteEntry(obj.GetType().Name, obj.GetType(), obj, writer);
				else
				{
					var elementWriter = GetSerializerForType(elementType);

					elementWriter.WriteObject(writer, obj);
				}

			writer.WriteEndElement();
		}

		private string FormatAsString(object value)
		{
			if (value == null)
				return string.Empty;

			var converter = _stringConverters.Retrieve(value.GetType(), () => x => x.ToString());
			
			return converter(value);
		}

		public object ReadObject(XmlNode node)
		{
			if (_type.IsSimpleType())
				return GetPropertyValue(_type, node, null);

			object result = ClassFactory.New(_type);

			foreach (XmlNode n in node.ChildNodes)
			{
				PropertyInfo prop = GetProperty(n.Name);
				if (prop != null)
				{
					object val = GetPropertyValue(prop.PropertyType, n, result);
					if (val != null)
						prop.SetValue(result, val, null);
				}

				FieldInfo field = GetField(_type, n.Name);
				if (field != null)
				{
					object val = GetPropertyValue(field.FieldType, n, result);
					if (val != null)
						field.SetValue(result, val);
				}
			}

			return result;
		}

		private PropertyInfo GetProperty(string name)
		{
			foreach (PropertyInfo propertyInfo in _properties)
			{
				if(propertyInfo.Name == name)
					return propertyInfo;
			}

			return null;
		}

		private FieldInfo GetField(Type t, string name)
		{
			foreach (FieldInfo fieldInfo in _fields)
			{
				if (fieldInfo.Name == name)
					return fieldInfo;
			}

			return null;
		}

		private object GetPropertyValue(Type type, XmlNode n, object parent)
		{
			if (n.ChildNodes.Count == 1 && n.ChildNodes[0] is XmlText)
			{
				if (type == typeof(string))
					return n.ChildNodes[0].InnerText;

				if (type.IsPrimitive || type == typeof(decimal))
					return Convert.ChangeType(n.ChildNodes[0].InnerText, type);

				if (type == typeof(Guid))
					return new Guid(n.ChildNodes[0].InnerText);

				if (type == typeof(DateTime))
					return XmlConvert.ToDateTime(n.ChildNodes[0].InnerText, XmlDateTimeSerializationMode.Utc);

				if (type == typeof(TimeSpan))
					return XmlConvert.ToTimeSpan(n.ChildNodes[0].InnerText);

				if (type.IsEnum)
					return Enum.Parse(type, n.ChildNodes[0].InnerText);
			}

			if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
			{
				Type typeToCreate = type;

				if (typeToCreate.GetGenericTypeDefinition() == typeof(IList<>))
					typeToCreate = typeof (List<>).MakeGenericType(typeToCreate.GetGenericArguments());

				IList list = Activator.CreateInstance(typeToCreate) as IList;

				foreach (XmlNode xn in n.ChildNodes)
				{
					var elementSerializer = GetSerializerForNode(xn);

					object m = elementSerializer.ReadObject(xn);

					if (list != null)
						list.Add(m);
				}

//				if (isArray)
//					return typeToCreate.GetMethod("ToArray").Invoke(list, null);

				return list;
			}

			if (n.ChildNodes.Count == 0)
				if (type == typeof(string))
					return string.Empty;
				else
					return null;

			var childSerializer = GetSerializerForNode(n);

			return childSerializer.ReadObject(n);
		}


		public static XmlObjectSerializer GetSerializerForNode(XmlNode node)
		{
			XmlAttribute typeAttribute = node.Attributes["type", "http://www.w3.org/2001/XMLSchema-instance"];
			if (typeAttribute != null)
			{
				var objectType = Type.GetType(typeAttribute.Value);
				if (objectType == null)
					throw new SerializationException("Unable to get type for " + typeAttribute.Value);

				return GetSerializerForType(objectType);
			}

			throw new SerializationException("Unable to get type for node: " + node.Name);
		}

		public static XmlObjectSerializer GetSerializerForType(Type type)
		{
			var properties = _propertiesForType.Retrieve(type, () => new TypePropertyInfo(type));
			var fields = _fieldsForType.Retrieve(type, () => new TypeFieldInfo(type));

			return new XmlObjectSerializer(type, properties, fields);
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
					type == typeof(string) ||
					type == typeof(decimal) ||
					type == typeof(Guid) ||
					type == typeof(DateTime) ||
					type == typeof(DateTimeOffset) ||
					type == typeof(TimeSpan) ||
					type.IsEnum);
		}
	}

}