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
	using Magnum.InterfaceExtensions;
	using Magnum.ObjectExtensions;
	using Magnum.Reflection;

	public class XmlObjectSerializer
	{
		private static readonly Dictionary<Type, Func<XmlReader, object>> _readerConverters;
		private static readonly Dictionary<Type, Func<object, string>> _stringConverters;

		[ThreadStatic]
		private static Dictionary<Type, TypeFieldInfo> _fieldsForType;

		[ThreadStatic]
		private static Dictionary<Type, TypePropertyInfo> _propertiesForType;

		private readonly TypeFieldInfo _fields;
		private readonly TypePropertyInfo _properties;
		private readonly Type _type;
		private static Dictionary<Type, Func<XmlReader, object>> _attributeConverters;

		static XmlObjectSerializer()
		{
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
					{typeof (string), x => (string) x}
				};

			_readerConverters = new Dictionary<Type, Func<XmlReader, object>>
				{
					{typeof (bool), x => x.ReadElementContentAsBoolean()},
					{typeof (byte), x => XmlConvert.ToByte(x.ReadElementContentAsString())},
					{typeof (char), x => XmlConvert.ToChar(x.ReadElementContentAsString())},
					{typeof (DateTime), x => x.ReadElementContentAsDateTime()},
					{typeof (DateTimeOffset), x => XmlConvert.ToDateTimeOffset(x.ReadElementContentAsString())},
					{typeof (decimal), x => x.ReadElementContentAsDecimal()},
					{typeof (double), x => x.ReadElementContentAsDouble()},
					{typeof (float), x => x.ReadElementContentAsFloat()},
					{typeof (Guid), x => XmlConvert.ToGuid(x.ReadElementContentAsString())},
					{typeof (int), x => x.ReadElementContentAsInt()},
					{typeof (long), x => x.ReadElementContentAsLong()},
					{typeof (sbyte), x => XmlConvert.ToSByte(x.ReadElementContentAsString())},
					{typeof (short), x => XmlConvert.ToInt16(x.ReadElementContentAsString())},
					{typeof (TimeSpan), x => XmlConvert.ToTimeSpan(x.ReadElementContentAsString())},
					{typeof (uint), x => XmlConvert.ToUInt32(x.ReadElementContentAsString())},
					{typeof (ulong), x => XmlConvert.ToUInt64(x.ReadElementContentAsString())},
					{typeof (ushort), x => XmlConvert.ToUInt16(x.ReadElementContentAsString())},
					{typeof (string), x => x.ReadElementContentAsString()}
				};			
			
			_attributeConverters = new Dictionary<Type, Func<XmlReader, object>>
				{
					{typeof (bool), x => x.ReadContentAsBoolean()},
					{typeof (byte), x => XmlConvert.ToByte(x.ReadContentAsString())},
					{typeof (char), x => XmlConvert.ToChar(x.ReadContentAsString())},
					{typeof (DateTime), x => x.ReadContentAsDateTime()},
					{typeof (DateTimeOffset), x => XmlConvert.ToDateTimeOffset(x.ReadContentAsString())},
					{typeof (decimal), x => x.ReadContentAsDecimal()},
					{typeof (double), x => x.ReadContentAsDouble()},
					{typeof (float), x => x.ReadContentAsFloat()},
					{typeof (Guid), x => XmlConvert.ToGuid(x.ReadContentAsString())},
					{typeof (int), x => x.ReadContentAsInt()},
					{typeof (long), x => x.ReadContentAsLong()},
					{typeof (sbyte), x => XmlConvert.ToSByte(x.ReadContentAsString())},
					{typeof (short), x => XmlConvert.ToInt16(x.ReadContentAsString())},
					{typeof (TimeSpan), x => XmlConvert.ToTimeSpan(x.ReadContentAsString())},
					{typeof (uint), x => XmlConvert.ToUInt32(x.ReadContentAsString())},
					{typeof (ulong), x => XmlConvert.ToUInt64(x.ReadContentAsString())},
					{typeof (ushort), x => XmlConvert.ToUInt16(x.ReadContentAsString())},
					{typeof (string), x => x.ReadContentAsString()}
				};
		}

		private XmlObjectSerializer(Type type, TypePropertyInfo properties, TypeFieldInfo fields)
		{
			_type = type;
			_properties = properties;
			_fields = fields;
		}

		private static Dictionary<Type, TypeFieldInfo> FieldsForType
		{
			get
			{
				if (_fieldsForType == null)
					_fieldsForType = new Dictionary<Type, TypeFieldInfo>();

				return _fieldsForType;
			}
		}

		private static Dictionary<Type, TypePropertyInfo> PropertiesForType
		{
			get
			{
				if (_propertiesForType == null)
					_propertiesForType = new Dictionary<Type, TypePropertyInfo>();

				return _propertiesForType;
			}
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

		public object ReadObject(XmlReader reader)
		{
			Func<XmlReader, object> converter;
			if (_readerConverters.TryGetValue(_type, out converter))
			{
				return converter(reader);
			}

			object result = ClassFactory.New(_type);

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					FastProperty prop = _properties.Get(reader.Name);
					if (prop != null)
					{
						object val = GetPropertyValue(prop.Property.PropertyType, reader, result);
						if (val != null)
							prop.Set(result, val);

						continue;
					}

					FieldInfo field = GetField(reader.Name);
					if (field != null)
					{
						object val = GetPropertyValue(field.FieldType, reader, result);
						if (val != null)
							field.SetValue(result, val);

						continue;
					}
				}
				else
				{
					throw new SerializationException("Unknown node type: " + reader.NodeType + " Name: " + reader.Name + " on type " + _type.FullName + " value " + reader.Value);
				}

				reader.Read();
			}

			// exit out of the EndElement
			reader.Read();

			return result;
		}

		private void Write(XmlWriter writer, object value)
		{
			if (value == null)
				return;

			foreach (FastProperty prop in _properties)
				WriteEntry(prop.Property.Name, prop.Property.PropertyType, prop.Get(value), writer);

			foreach (FieldInfo field in _fields)
				WriteEntry(field.Name, field.FieldType, field.GetValue(value), writer);
		}

		private void WriteEntry(string name, Type type, object value, XmlWriter writer)
		{
			if (value == null)
				return;

			Func<object, string> converter;
			if (_stringConverters.TryGetValue(type, out converter))
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
			writer.WriteAttributeString("xsi", "type", null, type.ToMessageName());

			Type[] genericArguments = type.GetDeclaredGenericArguments().ToArray();
			if (genericArguments != null && genericArguments.Length > 0)
			{
				if(value.Implements(typeof(IDictionary<,>)))
				{
					var keyType = genericArguments[0];
					var elementType = genericArguments[1];

					WriteEnumerableKeyValues(writer, value, keyType, elementType);
				}
				else if (value.Implements(typeof(IList<>)) || value.Implements(typeof(IEnumerable<>)))
				{
					WriteEnumerableValues(writer, value, genericArguments[0]);
				}
			}
			else
			{
				Type elementType =  type.IsArray ? type.GetElementType() : typeof(object);

				WriteEnumerableValues(writer, value, elementType);
			}

			writer.WriteEndElement();
		}

		private void WriteEnumerableValues(XmlWriter writer, object value, Type elementType)
		{
			foreach (object obj in ((IEnumerable) value))
				if (obj.GetType().IsSimpleType())
					WriteEntry(obj.GetType().Name, obj.GetType(), obj, writer);
				else
				{
					var elementWriter = GetSerializerForType(elementType);

					elementWriter.WriteObject(writer, obj);
				}
		}	
		
		private void WriteEnumerableKeyValues(XmlWriter writer, object value, Type keyType, Type elementType)
		{
			foreach (DictionaryEntry entry in ((IDictionary) value))
			{
				WriteKeyValuePair(writer, entry.Key, entry.Value);
			}
		}

		private void WriteKeyValuePair(XmlWriter writer, object key, object value)
		{
			writer.WriteStartElement("entry");

			Func<object, string> converter;
			Type keyType = key.GetType();

			if(_stringConverters.TryGetValue(keyType, out converter))
			{
				writer.WriteAttributeString("key", converter(key));
			}
			else
			{
				writer.WriteStartElement("key");

				GetSerializerForType(keyType).WriteObject(writer, key);
				writer.WriteEndElement();
			}

			Type valueType = value.GetType();
			if(_stringConverters.TryGetValue(valueType, out converter))
			{
				writer.WriteValue(converter(value));
			}
			else
			{
				GetSerializerForType(valueType).WriteObject(writer, value);
			}

			writer.WriteEndElement();
		}


		private FieldInfo GetField(string name)
		{
			foreach (FieldInfo fieldInfo in _fields)
			{
				if (fieldInfo.Name == name)
					return fieldInfo;
			}

			return null;
		}

		private static Type GetObjectType(XmlReader reader)
		{
			Type objectType = null;
			if (reader.HasAttributes &&
				reader.MoveToAttribute("type", "http://www.w3.org/2001/XMLSchema-instance"))
			{
				string typeName = reader.ReadContentAsString();

				objectType = Type.GetType(typeName);
			}

			reader.MoveToContent();
			return objectType;
		}


		public static XmlObjectSerializer GetSerializerFor(XmlReader reader)
		{
			if (!reader.HasAttributes ||
			    !reader.MoveToAttribute("type", "http://www.w3.org/2001/XMLSchema-instance"))
				throw new SerializationException("Unable to deserialize message body, no type information found");

			string typeName = reader.ReadContentAsString();

			var objectType = Type.GetType(typeName);
			if (objectType == null)
				throw new SerializationException("Unable to get type for " + typeName);

			reader.MoveToContent();

			return GetSerializerForType(objectType);
		}

		public static XmlObjectSerializer GetSerializerForType(Type type)
		{
			var properties = PropertiesForType.Retrieve(type, () => new TypePropertyInfo(type));
			var fields = FieldsForType.Retrieve(type, () => new TypeFieldInfo(type));

			return new XmlObjectSerializer(type, properties, fields);
		}

		private static object GetPropertyValue(Type type, XmlReader reader, object parent)
		{
			Func<XmlReader, object> converter;
			if (_readerConverters.TryGetValue(type, out converter))
			{
				return converter(reader);
			}

			if (type.IsEnum)
				return Enum.Parse(type, reader.ReadElementContentAsString());

			if (typeof (IEnumerable).IsAssignableFrom(type) && type != typeof (string))
			{
				XmlObjectSerializer elementSerializer = null;

				Type typeToCreate = type;
				if (type.IsArray)
				{
					typeToCreate = typeof(List<>).MakeGenericType(type.GetElementType());
					elementSerializer = GetSerializerForType(type.GetElementType());

					return GetListProperty(type, reader, typeToCreate, elementSerializer);
				}
				if (typeToCreate.IsGenericType)
				{
					Type genericTypeDefinition = typeToCreate.GetGenericTypeDefinition();
					if (genericTypeDefinition == typeof(IList<>))
					{
						Type[] arguments = typeToCreate.GetGenericArguments();

						typeToCreate = typeof (List<>).MakeGenericType(arguments);
						elementSerializer = GetSerializerForType(arguments[0]);

						return GetListProperty(type, reader, typeToCreate, elementSerializer);
					}

					if (genericTypeDefinition == typeof(IDictionary<,>))
					{
						Type[] arguments = typeToCreate.GetGenericArguments();

						typeToCreate = typeof(Dictionary<,>).MakeGenericType(arguments);


						return GetDictionaryProperty(reader, typeToCreate);
					}
				}

				return GetListProperty(type, reader, typeToCreate, elementSerializer);
			}

			var childSerializer = GetSerializerFor(reader);

			return childSerializer.ReadObject(reader);
		}

		private static object GetListProperty(Type type, XmlReader reader, Type typeToCreate, XmlObjectSerializer elementSerializer)
		{
			IList list = ClassFactory.New(typeToCreate) as IList;
			if (list == null)
				throw new SerializationException("Unable to create list " + typeToCreate.FullName);

			reader.Read();

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				object m = null;

				Type elementType = GetObjectType(reader);
				if(elementType != null)
				{
					m = GetSerializerForType(elementType).ReadObject(reader);
				}
				else if (elementSerializer != null)
				{
					m = elementSerializer.ReadObject(reader);
				}

				if (m != null)
					list.Add(m);
			}

			if (type.IsArray)
				return typeToCreate.GetMethod("ToArray").Invoke(list, null);

			return list;
		}

		private static object GetDictionaryProperty(XmlReader reader, Type typeToCreate)
		{
			IDictionary dictionary = ClassFactory.New(typeToCreate) as IDictionary;
			if (dictionary == null)
				throw new SerializationException("Unable to create dictionary " + typeToCreate.FullName);

			Type[] arguments = typeToCreate.GetGenericArguments();

			Func<XmlObjectSerializer> keySerializer = () => GetSerializerForType(arguments[0]);
			Func<XmlObjectSerializer> elementSerializer = () => GetSerializerForType(arguments[1]);

			Type keyType = arguments[0];
			Type valueType = arguments[1];

			reader.Read();

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				object key;

				if (reader.HasAttributes &&
					reader.MoveToAttribute("key"))
				{
					Func<XmlReader, object> converter;
					if (_attributeConverters.TryGetValue(keyType, out converter))
					{
						key = converter(reader);
					}
					else
					{
						throw new InvalidOperationException("Not sure how to handle this key type");
					}
					reader.Read();
				}
				else
				{
					reader.Read();

					key = keySerializer().ReadObject(reader);
				}

				Func<XmlReader, object> valueConverter;
				object value;
				if(_readerConverters.TryGetValue(valueType, out valueConverter))
				{
					value = valueConverter(reader);
				}
				else
				{
					value = elementSerializer().ReadObject(reader);
				}

				reader.Read(); // get past end element

				if ( key != null)
					dictionary.Add(key, value);
			}

			reader.Read(); // end element

			return dictionary;
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
			        type == typeof (DateTimeOffset) ||
			        type == typeof (TimeSpan) ||
			        type.IsEnum);
		}
	}
}