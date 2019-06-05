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


    /// <summary>
    /// A cache of convention-based CorrelationId mappers, used unless overridden by some mystical force
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScheduleTokenIdCache<T> :
        IScheduleTokenIdCache<T>
        where T : class
    {
        public delegate Guid? TokenIdSelector(T instance);


        readonly TokenIdSelector _selector;

        ScheduleTokenIdCache(TokenIdSelector selector)
        {
            _selector = selector;
        }

        ScheduleTokenIdCache()
        {
            _selector = x => default;
        }

        public bool TryGetTokenId(T message, out Guid tokenId)
        {
            Guid? result = _selector(message);
            if (result.HasValue)
            {
                tokenId = result.Value;
                return true;
            }

            tokenId = default;
            return false;
        }

        public static Guid GetTokenId(T message, Guid? defaultValue = default)
        {
            if (Cached.Metadata.Value.TryGetTokenId(message, out var tokenId))
                return tokenId;

            return defaultValue ?? NewId.NextGuid();
        }

        internal static void UseTokenId(TokenIdSelector tokenIdSelector)
        {
            if (Cached.Metadata.IsValueCreated)
                return;

            Cached.Metadata = new Lazy<IScheduleTokenIdCache<T>>(() => new ScheduleTokenIdCache<T>(tokenIdSelector));
        }


        static class Cached
        {
            internal static Lazy<IScheduleTokenIdCache<T>> Metadata = new Lazy<IScheduleTokenIdCache<T>>(
                () => new ScheduleTokenIdCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
