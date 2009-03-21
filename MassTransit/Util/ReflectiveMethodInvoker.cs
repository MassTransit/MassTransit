namespace MassTransit.Util
{
	using System;
	using System.Reflection;

	public static class ReflectiveMethodInvoker
	{
		public static MethodInfo FindMethod(Type type,
									   string methodName,
									   Type[] typeArguments,
									   Type[] parameterTypes)
		{
			MethodInfo methodInfo = null;

			if (null == parameterTypes)
			{
				methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				methodInfo = methodInfo.MakeGenericMethod(typeArguments);
			}
			else
			{
				// Method is probably overloaded. As far as I know there's no other way 
				// to get the MethodInfo instance, we have to
				// search for it in all the type methods
				MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				foreach (MethodInfo method in methods)
				{
					if (method.Name != methodName)
						continue;

					// create the generic method
					if (method.GetGenericArguments().Length != typeArguments.Length)
						continue;

					MethodInfo genericMethod = method.MakeGenericMethod(typeArguments);
					ParameterInfo[] parameters = genericMethod.GetParameters();

					// compare the method parameters
					if (parameters.Length != parameterTypes.Length)
						continue;

					if (!ParameterTypesAreCompatible(parameterTypes, parameters))
						continue;

					// if we're here, we got the right method
					methodInfo = genericMethod;
					break;
				}

				if (null == methodInfo)
				{
					throw new InvalidOperationException("Method not found");
				}
			}

			return methodInfo;
		}

		private static bool ParameterTypesAreCompatible(Type[] parameterTypes, ParameterInfo[] parameters)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].ParameterType == parameterTypes[i])
					continue;

				if (parameters[i].ParameterType.IsAssignableFrom(parameterTypes[i]))
					continue;

				return false;
			}

			return true;
		}
	}
}