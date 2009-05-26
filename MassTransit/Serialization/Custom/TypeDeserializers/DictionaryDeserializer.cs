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
namespace MassTransit.Serialization.Custom.TypeDeserializers
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

	public class DictionaryDeserializer<TKey, TValue> :
		IObjectDeserializer<IDictionary<TKey, TValue>>
	{
		public object Deserialize(IDeserializerContext context)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

			context.Read();

			while (context.NodeType != XmlNodeType.EndElement)
			{
				ReadItem(context, dictionary);
			}

			return dictionary;
		}

		private void ReadItem(IDeserializerContext context, IDictionary<TKey, TValue> dictionary)
		{
			if (context.NodeType != XmlNodeType.Element || context.LocalName != "item")
				throw new InvalidOperationException("Dictionary is not at an item element");

			context.Read();

			object key = context.Deserialize(context.Namespace);

			object element = context.Deserialize(context.Namespace);

			if (context.NodeType != XmlNodeType.EndElement || context.LocalName != "item")
				throw new InvalidOperationException("Dictionary is not at the end of an item");

			context.Read();

			if (key != null)
			{
				dictionary.Add((TKey) key, (TValue) element);
			}
		}
	}
}