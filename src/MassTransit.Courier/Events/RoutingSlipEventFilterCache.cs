// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier.Events
{
    using System;
    using System.Collections.Concurrent;
    using Contracts;
    using InternalMessages;


    public static class RoutingSlipEventFilterCache
    {
        public static T Filter<T>(T message, RoutingSlipEventContents contents)
            where T : class
        {
            if (contents == RoutingSlipEventContents.All)
                return message;

            IRoutingSlipEventFilter filter;
            if (InstanceCache.Cached.TryGetValue(typeof(T), out filter))
                return filter.Filter(message, contents);

            return message;
        }


        static class InstanceCache
        {
            internal static readonly ConcurrentDictionary<Type, IRoutingSlipEventFilter> Cached =
                new ConcurrentDictionary<Type, IRoutingSlipEventFilter>();

            static InstanceCache()
            {
                Cached.GetOrAdd(typeof(RoutingSlipCompleted), x => new RoutingSlipCompletedEventFilter());
                Cached.GetOrAdd(typeof(RoutingSlipFaulted), x => new RoutingSlipFaultedEventFilter());
                Cached.GetOrAdd(typeof(RoutingSlipActivityCompleted), x => new RoutingSlipActivityCompletedEventFilter());
                Cached.GetOrAdd(typeof(RoutingSlipActivityFaulted), x => new RoutingSlipActivityFaultedEventFilter());
                Cached.GetOrAdd(typeof(RoutingSlipActivityCompensated), x => new RoutingSlipActivityCompensatedEventFilter());
                Cached.GetOrAdd(typeof(CompensationFailed), x => new RoutingSlipCompensationFailedEventFilter());
            }
        }
    }
}