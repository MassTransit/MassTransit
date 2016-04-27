// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using System.Threading;
    using Internals.Extensions;


    /// <summary>
    /// A cache of convention-based CorrelationId mappers, used unless overridden by some mystical force
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageCorrelationCache<T> :
        IMessageCorrelationCache<T>
        where T : class
    {
        readonly Lazy<ISetCorrelationId<T>> _setCorrelationIdPipe;

        MessageCorrelationCache()
        {
            _setCorrelationIdPipe = new Lazy<ISetCorrelationId<T>>(CreateCorrelationPipe);
        }

        MessageCorrelationCache(ISetCorrelationId<T> setCorrelationId)
        {
            _setCorrelationIdPipe = new Lazy<ISetCorrelationId<T>>(() => setCorrelationId);
        }

        void IMessageCorrelationCache<T>.SetCorrelationId(SendContext<T> context)
        {
            _setCorrelationIdPipe.Value.SetCorrelationId(context);
        }

        public static void SetCorrelationId(SendContext<T> context)
        {
            Cached.Metadata.Value.SetCorrelationId(context);
        }

        internal static void UseCorrelationId(Func<T, Guid> getCorrelationId)
        {
            if (Cached.Metadata.IsValueCreated)
                throw new InvalidOperationException("The correlationId pipe has already been created");

            Cached.Metadata = new Lazy<IMessageCorrelationCache<T>>(
                () => new MessageCorrelationCache<T>(new DelegateSetCorrelationId<T>(context => getCorrelationId(context))));
        }

        ISetCorrelationId<T> CreateCorrelationPipe()
        {
            var correlatedByInterface = typeof(T).GetInterface<CorrelatedBy<Guid>>();
            if (correlatedByInterface != null)
            {
                var objectType = typeof(CorrelatedBySetCorrelationId<>).MakeGenericType(typeof(T));
                return (ISetCorrelationId<T>)Activator.CreateInstance(objectType);
            }

            var propertyInfo = typeof(T).GetProperty("CorrelationId");
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid))
            {
                var objectType = typeof(PropertySetCorrelationId<>).MakeGenericType(typeof(T));
                return (ISetCorrelationId<T>)Activator.CreateInstance(objectType, propertyInfo);
            }

            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid?))
            {
                var objectType = typeof(NullablePropertySetCorrelationId<>).MakeGenericType(typeof(T));
                return (ISetCorrelationId<T>)Activator.CreateInstance(objectType, propertyInfo);
            }

            propertyInfo = typeof(T).GetProperty("EventId");
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid))
            {
                var objectType = typeof(PropertySetCorrelationId<>).MakeGenericType(typeof(T));
                return (ISetCorrelationId<T>)Activator.CreateInstance(objectType, propertyInfo);
            }

            propertyInfo = typeof(T).GetProperty("CommandId");
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid))
            {
                var objectType = typeof(PropertySetCorrelationId<>).MakeGenericType(typeof(T));
                return (ISetCorrelationId<T>)Activator.CreateInstance(objectType, propertyInfo);
            }

            return new NoSetCorrelationId<T>();
        }


        static class Cached
        {
            internal static Lazy<IMessageCorrelationCache<T>> Metadata = new Lazy<IMessageCorrelationCache<T>>(
                () => new MessageCorrelationCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}