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
namespace MassTransit.Logging
{
    using System;
    using Tracing;
    using Util;


    public static class Logger
    {
        static ILogger _logger;

        public static ILogger Current => _logger ?? (_logger = new TraceLogger());

        public static ILog Get<T>()
            where T : class
        {
            return Get(TypeMetadataCache<T>.ShortName);
        }

        public static ILog Get(Type type)
        {
            return Get(TypeMetadataCache.GetShortName(type));
        }

        public static ILog Get(string name)
        {
            return Current.Get(name);
        }

        public static void UseLogger(ILogger logger)
        {
            _logger = logger;
        }
    }
}