// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context.Converters
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using Metadata;
    using Util;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class ConsumeObserverConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IConsumeObserverConverter>> _types = new ConcurrentDictionary<Type, Lazy<IConsumeObserverConverter>>();

        IConsumeObserverConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task PreConsume(Type messageType, IConsumeObserver observer, object context)
        {
            return Cached.Converters.Value[messageType].PreConsume(observer, context);
        }

        public static Task PostConsume(Type messageType, IConsumeObserver observer, object context)
        {
            return Cached.Converters.Value[messageType].PostConsume(observer, context);
        }

        public static Task ConsumeFault(Type messageType, IConsumeObserver observer, object context, Exception exception)
        {
            return Cached.Converters.Value[messageType].ConsumeFault(observer, context, exception);
        }

        static Lazy<IConsumeObserverConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<IConsumeObserverConverter>(() => CreateConverter(type));
        }

        static IConsumeObserverConverter CreateConverter(Type type)
        {
            if (type.ClosesType(typeof(ConsumeContext<>)))
            {
                var messageType = type.GetClosingArguments(typeof(ConsumeContext<>)).Single();

                var converterType = typeof(ConsumeObserverConverter<>).MakeGenericType(messageType);

                return (IConsumeObserverConverter)Activator.CreateInstance(converterType);
            }

            throw new ArgumentException($"The context was not a ConsumeContext: {TypeMetadataCache.GetShortName(type)}", nameof(type));
        }


        static class Cached
        {
            internal static readonly Lazy<ConsumeObserverConverterCache> Converters =
                new Lazy<ConsumeObserverConverterCache>(() => new ConsumeObserverConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}