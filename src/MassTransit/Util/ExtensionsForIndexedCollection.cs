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
namespace MassTransit.Util
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class ExtensionsForIndexedCollection
	{
		public static IEnumerable<T> Where<T>(this IndexedCollection<T> collection, Expression<Func<T, bool>> expression)
		{
			if (expression.Body.NodeType == ExpressionType.Equal)
			{
				var binaryExpression = (BinaryExpression) expression.Body;

				int? hashRight = GetHashRight(binaryExpression.Right);

				PropertyInfo propertyInfo;
				if (TryGetIndexedProperty(binaryExpression.Left, collection, out propertyInfo))
				{
					Dictionary<int, List<T>> index = collection.GetIndexByProperty(propertyInfo.Name);
					if (index.ContainsKey(hashRight.Value))
					{
						foreach (T item in index[hashRight.Value].Where(expression.Compile()))
						{
							yield return item;
						}

						yield break;
					}
				}
			}

			foreach (T item in collection.Where(expression.Compile()))
			{
				yield return item;
			}
		}

		private static bool TryGetIndexedProperty<T>(Expression left, IndexedCollection<T> collection, out PropertyInfo propertyInfo)
		{
			if (left.NodeType == ExpressionType.MemberAccess)
			{
				propertyInfo = ((MemberExpression) left).Member as PropertyInfo;

				return propertyInfo != null && collection.PropertyHasIndex(propertyInfo.Name);
			}

			propertyInfo = null;
			return false;
		}

		private static int? GetHashRight(Expression right)
		{
			switch (right.NodeType)
			{
				case ExpressionType.Constant:
					return ((ConstantExpression) right).Value.GetHashCode();

				default:
					return Expression.Lambda(right, null).Compile().DynamicInvoke(null).GetHashCode();
			}
		}
	}
}