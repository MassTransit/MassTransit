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
    using Microsoft.Extensions.Logging;
    using Tracing;


    public static class Logger
    {
        static ILoggerFactory _loggerFactory;

        static ILoggerFactory Current => _loggerFactory ?? (_loggerFactory = new TraceLoggerFactory());

        public static ILogger<T> Get<T>()
            where T : class =>
            Current.CreateLogger<T>();

        public static ILogger Get(Type type) => Current.CreateLogger(type);

        public static ILogger Get(string name) => Current.CreateLogger(name);

        public static void UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public static void Shutdown()
        {
            try
            {
                Current.Dispose();
            }
            catch (Exception e)
            {
                //ignore
            }
        }
    }
}
