// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Monitoring.Performance
{
    using System;


    public static class MessagePerformanceCounterCache<T>
    {
        static Lazy<MessagePerformanceCounter<T>> _cache;

        public static IMessagePerformanceCounter Counter(ICounterFactory factory)
        {
            if (_cache == null)
            {
                _cache = new Lazy<MessagePerformanceCounter<T>>(() => new MessagePerformanceCounter<T>(factory));
            }
            return _cache.Value;
        }
    }
}
