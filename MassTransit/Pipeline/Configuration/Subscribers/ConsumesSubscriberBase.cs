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
    using System.Linq.Expressions;
    using System.Reflection;
    using Exceptions;
    using Magnum.Threading;

    public abstract class ConsumesSubscriberBase<TInterceptor> :
        PipelineSubscriberBase
        where TInterceptor : class
    {
        private readonly ReaderWriterLockedDictionary<Type, Func<TInterceptor, ISubscriberContext, UnsubscribeAction>> _components;
        private readonly ReaderWriterLockedDictionary<Type, Func<TInterceptor, ISubscriberContext, object, UnsubscribeAction>> _instances;

        protected ConsumesSubscriberBase()
        {
            _components = new ReaderWriterLockedDictionary<Type, Func<TInterceptor, ISubscriberContext, UnsubscribeAction>>();
            _instances = new ReaderWriterLockedDictionary<Type, Func<TInterceptor, ISubscriberContext, object, UnsubscribeAction>>();
        }

        protected abstract Type InterfaceType { get; }

        public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context)
        {
            Func<TInterceptor, ISubscriberContext, UnsubscribeAction> invoker = GetInvoker<TComponent>();
            if (invoker == null)
                yield break;

            foreach (Func<TInterceptor, ISubscriberContext, UnsubscribeAction> action in invoker.GetInvocationList())
            {
                yield return action(this.TranslateTo<TInterceptor>(), context);
            }
        }

        public override IEnumerable<UnsubscribeAction> Subscribe<TComponent>(ISubscriberContext context, TComponent instance)
        {
            Func<TInterceptor, ISubscriberContext, object, UnsubscribeAction> invoker = GetInvokerForInstance<TComponent>();
            if (invoker == null)
                yield break;

            foreach (Func<TInterceptor, ISubscriberContext, object, UnsubscribeAction> action in invoker.GetInvocationList())
            {
                yield return action(this.TranslateTo<TInterceptor>(), context, instance);
            }
        }

        private Func<TInterceptor, ISubscriberContext, UnsubscribeAction> GetInvoker<TComponent>()
        {
            Type componentType = typeof (TComponent);

            return _components.Retrieve(componentType, () =>
            {
                Func<TInterceptor, ISubscriberContext, UnsubscribeAction> invoker = null;

                foreach (Type interfaceType in componentType.GetInterfaces())
                {
                    Type messageType = GetMessageType(interfaceType);
                    if (messageType == null)
                        continue;

                    MethodInfo genericMethod = FindMethod(GetType(), "Connect", new[] {typeof (TComponent), messageType}, new[] {typeof (ISubscriberContext)});
                    if (genericMethod == null)
                        throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
                                                                  typeof (TComponent).FullName, messageType.FullName));

                    var interceptorParameter = Expression.Parameter(typeof (TInterceptor), "interceptor");
                    var contextParameter = Expression.Parameter(typeof (ISubscriberContext), "context");

                    var call = Expression.Call(interceptorParameter, genericMethod, contextParameter);

                    var connector = Expression.Lambda<Func<TInterceptor, ISubscriberContext, UnsubscribeAction>>(call, new[] { interceptorParameter, contextParameter }).Compile();

                    if (invoker == null)
                    {
                        invoker = (interceptor, context) =>
                        {
                            if (context.HasMessageTypeBeenDefined(messageType))
                                return () => true;

                            context.MessageTypeWasDefined(messageType);
                            return connector(interceptor, context);
                        };
                    }
                    else
                    {
                        invoker += (interceptor, context) =>
                        {
                            if (context.HasMessageTypeBeenDefined(messageType))
                                return () => true;

                            context.MessageTypeWasDefined(messageType);
                            return connector(interceptor, context);
                        };
                    }
                }

                return invoker;
            });
        }

        private Type GetMessageType(Type interfaceType)
        {
            if (!interfaceType.IsGenericType)
                return null;

            Type genericType = interfaceType.GetGenericTypeDefinition();
            if (genericType != InterfaceType)
                return null;

            Type[] types = interfaceType.GetGenericArguments();

            Type messageType = types[0];

            return messageType;
        }


        private Func<TInterceptor, ISubscriberContext, object, UnsubscribeAction> GetInvokerForInstance<TComponent>()
        {
            Type componentType = typeof (TComponent);

            return _instances.Retrieve(componentType, () =>
            {
                Func<TInterceptor, ISubscriberContext, object, UnsubscribeAction> invoker = null;

                // since we don't have it, we're going to build it

                foreach (Type interfaceType in componentType.GetInterfaces())
                {
                    Type messageType = GetMessageType(interfaceType);
                    if (messageType == null)
                        continue;

                    MethodInfo genericMethod = FindMethod(GetType(), "Connect", new[] {messageType}, new[] {typeof (ISubscriberContext), typeof (TComponent)});
                    if (genericMethod == null)
                        throw new PipelineException(string.Format("Unable to subscribe for type: {0} ({1})",
                                                                  typeof (TComponent).FullName, messageType.FullName));

                    var interceptorParameter = Expression.Parameter(typeof (TInterceptor), "interceptor");
                    var contextParameter = Expression.Parameter(typeof (ISubscriberContext), "context");
                    var instanceParameter = Expression.Parameter(typeof (object), "instance");

                    var instanceCast = Expression.Convert(instanceParameter, typeof (TComponent));

                    var call = Expression.Call(interceptorParameter, genericMethod, contextParameter, instanceCast);

                    var connector = Expression.Lambda<Func<TInterceptor, ISubscriberContext, object, UnsubscribeAction>>(call, new[] { interceptorParameter, contextParameter, instanceParameter }).Compile();

                    if (invoker == null)
                    {
                        invoker = (interceptor, context, obj) =>
                        {
                            if (context.HasMessageTypeBeenDefined(messageType))
                                return () => true;

                            context.MessageTypeWasDefined(messageType);

                            return connector(interceptor, context, obj);
                        };
                    }
                    else
                    {
                        invoker += (interceptor, context, obj) =>
                        {
                            if (context.HasMessageTypeBeenDefined(messageType))
                                return () => true;

                            context.MessageTypeWasDefined(messageType);

                            return connector(interceptor, context, obj);
                        };
                    }
                }

                return invoker;
            });
        }
    }
}