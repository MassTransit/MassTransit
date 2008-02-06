/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.Host
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using ServiceBus;

    public class ClassSubscriptionManager :
        IClassSubscriptionManager
    {
        #region IClassSubscriptionManager Members

        public IList<Type> FindMessageTypes(Type classType)
        {
            List<Type> types = new List<Type>();

            MethodInfo[] methods = classType.GetMethods();
            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] parms = method.GetParameters();
                if (parms.Length != 1)
                    continue;

                foreach (ParameterInfo parm in parms)
                {
                    if (parm.ParameterType.IsGenericType)
                    {
                        Type[] genericArgs = parm.ParameterType.GetGenericArguments();

                        if (genericArgs.Length == 1)
                        {
                            if (typeof (IMessage).IsAssignableFrom(genericArgs[0]))
                            {
                                Type handlerType = typeof (MessageContext<>).MakeGenericType(genericArgs[0]);
                                if (handlerType.IsAssignableFrom(parm.ParameterType))
                                {
                                    types.Add(genericArgs[0]);
                                }
                            }
                        }
                    }
                }
            }

            return types;
        }

        public void SubscribeHandlers(IServiceBus bus, IAutoSubscriber autoSubscriber)
        {
            autoSubscriber.AddSubscriptions(bus);
        }

        #endregion
    }
}