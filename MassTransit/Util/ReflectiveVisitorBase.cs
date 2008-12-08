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
	using System.Reflection;

	public abstract class ReflectiveVisitorBase
	{
		private readonly string _methodName;
		private const BindingFlags _bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

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

			while (objectType != typeof(object))
			{
				bool result;
				if (SimpleDispatch(obj, objectType, out result))
					return result;

				if (GenericDispatch(obj, objectType, out result))
					return result;

				objectType = objectType.BaseType;
			}

			// if we are here, we need to think about maybe doing interfaces
			foreach (Type interfaceType in obj.GetType().GetInterfaces())
			{
				bool result;
				if (SimpleDispatch(obj, interfaceType, out result))
					return result;

				if (GenericDispatch(obj, interfaceType, out result))
					return result;
			}

			return true;
		}

		private bool SimpleDispatch(object obj, Type objectType, out bool result)
		{
			result = true;

			Type[] argumentTypes = new[] {objectType};

			MethodInfo mi = GetType().GetMethod(_methodName, _bindingFlags, null, argumentTypes, null);
			if (mi == null)
				return false;

			if (mi.GetParameters()[0].ParameterType == typeof (object))
				return false;

			result = (bool) mi.Invoke(this, new[] {obj});

			return true;
		}

		private bool GenericDispatch(object obj, Type objectType, out bool result)
		{
			result = true;

			if (!objectType.IsGenericType)
				return false;

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
				return false;

			result = (bool) match.Invoke(this, new[] {obj});

			return true;
		}
	}
}