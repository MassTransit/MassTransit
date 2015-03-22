// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using Context;
	using Magnum.Extensions;
	using MassTransit.Transports;
	using MassTransit.Transports.InMemory;
	using NUnit.Framework;

	public static class TestExtensions
	{
		public static void ShouldBeEqualTo<T>(this T target, object expected)
			where T : class
		{
			Type targetType = typeof (T);

			CompareObjects(target, expected, targetType, null);
		}

		public static void PropertiesShouldMatch(Type target, Type expected)
		{
			target.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Each(property => { Assert.IsNotNull(expected.GetProperty(property.Name)); });
		}

		public static void PropertiesShouldNotMatch(Type target, Type expected)
		{
			bool mismatch = false;
			target.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Each(property => { if (expected.GetProperty(property.Name) == null) mismatch = true; });

			Assert.IsTrue(mismatch);
		}

		public static SendContext<T> ToSendContext<T>(this T obj)
			where T : class
		{
			return new InMemorySendContext<T>(obj);
		}

		static void CompareObjects(object target, object expected, Type targetType, string parentPrefix)
		{
			if (target == null && expected == null)
				return;

			if (expected == null)
				Assert.Fail((parentPrefix ?? targetType.Name) + ": Expected null, but target is not null");

			if (target == null)
				Assert.Fail((parentPrefix ?? targetType.Name) + ": Expected not null, but target is null");

			if (expected.GetType() != targetType)
				Assert.Fail((parentPrefix ?? targetType.Name) + ": Expected instance of type: " + expected.GetType() + " but was " +
				            targetType);

			targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Each(property =>
					{
						CompareProperty(property, target, expected, targetType, parentPrefix ?? targetType.Name);
						return;
					});
		}

		static void CompareProperty(PropertyInfo property, object target, object expected, Type targetType,
		                            string parentPrefix)
		{
			object targetValue = property.GetGetMethod().Invoke(target, null);
			object expectedValue = property.GetGetMethod().Invoke(expected, null);

			string prefix = "For " + parentPrefix + "." + property.Name + ", ";

			if (targetValue == null && expectedValue == null)
				return;

			if (expectedValue == null)
				Assert.Fail(prefix + "expected null, but target is not null");

			if (targetValue == null)
				Assert.Fail(prefix + "expected not null, but target is null");

			if (property.PropertyType.IsValueType || property.PropertyType == typeof (string))
			{
				if (!expectedValue.Equals(targetValue))
				{
					Assert.Fail(prefix + "expected \"" + GetDisplayValue(expectedValue) + "\", was \"" + GetDisplayValue(targetValue) +
					            "\"");
				}
			}
			else if (typeof (IEnumerable).IsAssignableFrom(property.PropertyType))
			{
				if (property.PropertyType.GetGenericTypeDefinition() == typeof (IList<>))
				{
					Type elementType = property.PropertyType.GetGenericArguments()[0];

					CompareCollections(targetValue, expectedValue, property.PropertyType, elementType,
						parentPrefix + "." + property.Name);
				}
				else
				{
					throw new InvalidOperationException("Unable to handle collection type" + targetType);
				}
			}
			else
			{
				CompareObjects(targetValue, expectedValue, targetValue.GetType(), parentPrefix + "." + property.Name);
			}
		}

		static void CompareCollections(object target, object expected, Type targetType, Type elementType, string parentPrefix)
		{
			var left = target as IEnumerable;
			var right = expected as IEnumerable;

			IEnumerator e1 = left.GetEnumerator();
			IEnumerator e2 = right.GetEnumerator();
			{
				int i = 0;

				while (e1.MoveNext())
				{
					if (!e2.MoveNext())
						throw new InvalidOperationException("More items are present than expected in " + parentPrefix);

					CompareObjects(e1.Current, e2.Current, elementType, parentPrefix + "[" + i + "]");

					i++;
				}
				if (e2.MoveNext())
					throw new InvalidOperationException("There were less items than expected in " + parentPrefix);
			}
		}

		static string GetDisplayValue(object value)
		{
			if (value == null)
				return "(null)";

			return value.ToString();
		}
	}
}