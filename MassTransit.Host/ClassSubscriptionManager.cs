using System;
using System.Collections.Generic;
using System.Reflection;
using MassTransit.ServiceBus;

namespace MassTransit.Host
{
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