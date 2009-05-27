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
	using System.Collections.Generic;
	using System.Xml;

	public abstract class EnumerableDeserializerBase<T> :
		IObjectDeserializer<T>
	{
		public abstract object Deserialize(IDeserializerContext context);

		public virtual List<T> DeserializeAsList(IDeserializerContext context)
		{
			List<T> list = new List<T>();

			if (ListIsEmpty(context))
				return list;

			MoveToFirstElement(context);

			while (context.NodeType != XmlNodeType.EndElement)
			{
				object element = context.Deserialize(context.Namespace);
				if (element != null)
				{
					list.Add((T) element);
				}
			}

			MovePastEndElement(context);

			return list;
		}

		private bool ListIsEmpty(IDeserializerContext context)
		{
			if (context.IsEmptyElement)
			{
				context.Read();
				return true;
			}

			return false;
		}

		private void MovePastEndElement(IDeserializerContext context)
		{
			context.Read();
		}

		private void MoveToFirstElement(IDeserializerContext context)
		{
			context.Read();
		}
	}
}