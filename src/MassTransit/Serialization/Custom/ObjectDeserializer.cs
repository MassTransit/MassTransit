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
	using System.Xml;
	using log4net;
	using Magnum.Reflection;
	using Util;

	public class ObjectDeserializer<T> :
		IObjectDeserializer<T>
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ObjectDeserializer<T>));

		private readonly IDeserializeObjectPropertyCache<T> _propertyCache;

		public ObjectDeserializer()
		{
			_propertyCache = new DeserializeObjectPropertyCache<T>();
		}

		public object Deserialize(IDeserializerContext context)
		{
			T instance = FastActivator<T>.Create();

			if (context.IsEmptyElement)
			{
				context.Read();
				return instance;
			}

			if (!context.Read())
				return instance;

			while (context.NodeType != XmlNodeType.EndElement && context.NodeType != XmlNodeType.None)
			{
				if (context.NodeType == XmlNodeType.Element)
				{
					ReadProperty(context, instance);
					continue;
				}

				if (!context.Read())
					break;
			}

			context.ExitElement();

			return instance;
		}

		private void ReadProperty(IDeserializerContext context, T instance)
		{
			DeserializeObjectProperty<T> property;
			if (_propertyCache.TryGetProperty(context.LocalName, out property))
			{
				object value = context.Deserialize(context.Namespace);

				property.SetValue(instance, value);
			}
			else
			{
				if (_log.IsDebugEnabled)
				{
					_log.Debug("No property " + context.LocalName + " in class " + typeof (T).ToFriendlyName() + " for deserialization");
				}

				context.Deserialize(context.Namespace);
			}
		}
	}
}