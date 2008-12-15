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
	using System.Linq.Expressions;
	using System.Reflection;

	public abstract class ReflectiveVisitorBase<TVisitor> 
		where TVisitor : class
	{
		private readonly string _methodName;
		private const BindingFlags _bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

		private static readonly ReaderWriterLockedDictionary<Type, Func<TVisitor, object, bool>> _types = new ReaderWriterLockedDictionary<Type, Func<TVisitor, object, bool>>();

		protected ReflectiveVisitorBase()
		{
			_methodName = "Visit";
		}

		protected ReflectiveVisitorBase(string methodName)
		{
			_methodName = methodName;
		}

		public virtual bool Visit(object obj)
		{
			return DispatchVisit(obj);
		}

		public virtual bool Visit(object obj, Func<bool> action)
		{
			if (!DispatchVisit(obj))
				return false;

			IncreaseDepth();
			bool result = action();
			DecreaseDepth();

			return result;
		}

		protected virtual void IncreaseDepth()
		{
		}

		protected virtual void DecreaseDepth()
		{
		}

		protected bool DispatchVisit(object obj)
		{
			Type objectType = obj.GetType();

			Func<TVisitor, object,bool> method = _types.Retrieve(objectType, () =>
				{
					while (objectType != typeof (object))
					{
						Func<TVisitor, object, bool> result = SimpleDispatch(obj, objectType);
						if(result != null)
							return result;

						result = GenericDispatch(obj, objectType);
						if(result != null)
							return result;

						objectType = objectType.BaseType;
					}

					// if we are here, we need to think about maybe doing interfaces
					foreach (Type interfaceType in obj.GetType().GetInterfaces())
					{
						Func<TVisitor, object, bool> result = SimpleDispatch(obj, interfaceType);
						if(result != null)
							return result;

						result = GenericDispatch(obj, interfaceType);
						if(result != null)
							return result;
					}

					return null;
				});

			return method == null || method(this as TVisitor, obj);
		}

		private Func<TVisitor, object, bool> SimpleDispatch(object obj, Type objectType)
		{
			Type[] argumentTypes = new[] {objectType};

			MethodInfo mi = GetType().GetMethod(_methodName, _bindingFlags, null, argumentTypes, null);
			if (mi == null)
				return null;

			if (mi.GetParameters()[0].ParameterType == typeof (object))
				return null;

			return GenerateLambda(objectType, mi);
		}

		private Func<TVisitor, object, bool> GenerateLambda(Type objectType, MethodInfo mi)
		{
			var instance = Expression.Parameter(typeof(TVisitor), "visitor");
			var value = Expression.Parameter(typeof(object), "value");

			UnaryExpression valueCast = Expression.TypeAs(value, objectType);

			var del = Expression.Lambda<Func<TVisitor, object, bool>>(Expression.Call(instance, mi, valueCast), new[] { instance, value }).Compile();

			return del;
		}

		private Func<TVisitor, object, bool> GenericDispatch(object obj, Type objectType)
		{
			if (!objectType.IsGenericType)
				return null;

			Type[] genericArguments = objectType.GetGenericArguments();

			MethodInfo match = null;

			MethodInfo[] methods = GetType().GetMethods(_bindingFlags);

			foreach (MethodInfo method in methods)
			{
				if (method.Name != _methodName)
					continue;

				Type[] methodArguments = method.GetGenericArguments();

				if (methodArguments.Length != genericArguments.Length)
					continue;

				ParameterInfo[] methodParameters = method.GetParameters();

				if (methodParameters.Length != 1)
					continue;

				Type genericTypeDefinition = objectType.GetGenericTypeDefinition();
				Type typeDefinition = methodParameters[0].ParameterType.GetGenericTypeDefinition();

				if (typeDefinition != genericTypeDefinition)
					continue;

				MethodInfo genericMethod = method.MakeGenericMethod(genericArguments);

				ParameterInfo[] parameters = genericMethod.GetParameters();

				if (parameters.Length != 1)
					continue;

				if (parameters[0].ParameterType != objectType)
					continue;

				match = genericMethod;
				break;
			}

			if (match == null)
				return null;

			return GenerateLambda(objectType, match);
		}
	}
}