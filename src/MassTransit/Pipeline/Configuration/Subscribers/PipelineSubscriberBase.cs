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
namespace MassTransit.Pipeline.Configuration.Subscribers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class PipelineSubscriberBase :
        IPipelineSubscriber
    {
        public abstract IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context);
        public abstract IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context, TComponent instance);

        /// <summary>
        /// Returns the method information for the requested type and arguments
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="typeArguments"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        protected static MethodInfo FindMethod(Type type,
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

    	bool _disposed;

    	public void Dispose()
    	{
    		Dispose(true);
    		GC.SuppressFinalize(this);
    	}

    	~PipelineSubscriberBase()
    	{
    		Dispose(false);
    	}

    	protected virtual void Dispose(bool disposing)
    	{
    		if (_disposed) return;
    		if (disposing)
    		{
    			
    		}

    		_disposed = true;
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