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
    using System.Threading.Tasks;
    using Metadata;
    using Util;


    /// <summary>
    /// Converts the object message type to the generic type T and publishes it on the endpoint specified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsumeObserverConverter<T> :
        IConsumeObserverConverter
        where T : class
    {
        Task IConsumeObserverConverter.PreConsume(IConsumeObserver observer, object context)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var consumeContext = context as ConsumeContext<T>;
            if (consumeContext == null)
                throw new ArgumentException("Unexpected context type: " + TypeMetadataCache.GetShortName(context.GetType()));

            return observer.PreConsume(consumeContext);
        }

        Task IConsumeObserverConverter.PostConsume(IConsumeObserver observer, object context)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var consumeContext = context as ConsumeContext<T>;
            if (consumeContext == null)
                throw new ArgumentException("Unexpected context type: " + TypeMetadataCache.GetShortName(context.GetType()));

            return observer.PostConsume(consumeContext);
        }

        Task IConsumeObserverConverter.ConsumeFault(IConsumeObserver observer, object context, Exception exception)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var consumeContext = context as ConsumeContext<T>;
            if (consumeContext == null)
                throw new ArgumentException("Unexpected context type: " + TypeMetadataCache.GetShortName(context.GetType()));

            return observer.ConsumeFault(consumeContext, exception);
        }
    }
}