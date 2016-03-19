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
namespace MassTransit.Scheduling
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class MessageSchedulerConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IMessageSchedulerConverter>> _types =
            new ConcurrentDictionary<Type, Lazy<IMessageSchedulerConverter>>();

        IMessageSchedulerConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
            Type messageType, CancellationToken cancellationToken)
        {
            return Cached.Converters.Value[messageType].ScheduleSend(scheduler, destinationAddress, scheduledTime, message, cancellationToken);
        }

        public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
            Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return Cached.Converters.Value[messageType].ScheduleSend(scheduler, destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        static Lazy<IMessageSchedulerConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<IMessageSchedulerConverter>(() => CreateConverter(type));
        }

        static IMessageSchedulerConverter CreateConverter(Type type)
        {
            var converterType = typeof(MessageSchedulerConverter<>).MakeGenericType(type);

            return (IMessageSchedulerConverter)Activator.CreateInstance(converterType);
        }


        static class Cached
        {
            internal static readonly Lazy<MessageSchedulerConverterCache> Converters =
                new Lazy<MessageSchedulerConverterCache>(() => new MessageSchedulerConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}