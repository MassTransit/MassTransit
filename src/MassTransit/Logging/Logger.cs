// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Diagnostics;
    using Tracing;

    public static class Logger
    {
        static ILogger _logger;

        public static ILogger Current
        {
            get { return _logger ?? (_logger = new TraceLogger()); }
        }

        public static ILog Get(Type type)
        {
            return Get(type.FullName);
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

    public interface ILogWriter
    {
    }

    public class TraceLogWriter :
        ILogWriter
    {
        const string NullString = "null";
        readonly Func<bool> _enabled;

        public TraceLogWriter(Func<bool> enabled)
        {
            _enabled = enabled;
        }

        public void Write(string format, params object[] args)
        {
            if (_enabled())
                Trace.WriteLine(string.Format(format, args));
        }

        public void Write(IFormatProvider provider, string format, params object[] args)
        {
            if (_enabled())
                Trace.WriteLine(string.Format(provider, format, args));
        }

        public void Write(Exception exception, string format, params object[] args)
        {
            if (_enabled())
            {
                Trace.WriteLine(string.Format(format, args));
                Trace.WriteLine(exception);
            }
        }

        public void Write(Exception exception, IFormatProvider provider, string format, params object[] args)
        {
            if (_enabled())
            {
                Trace.WriteLine(string.Format(provider, format, args));
                Trace.WriteLine(exception);
            }
        }

        public void Write(Action<ILogWriter> action)
        {
            if (_enabled())
                action(this);
        }

        public void Write(object obj)
        {
            if (_enabled())
                Trace.WriteLine(obj != null ? obj.ToString() : NullString);
        }

        public void Write(string message)
        {
            if (_enabled() && !string.IsNullOrEmpty(message))
                Trace.WriteLine(message);
        }

        public void Write(Exception exception)
        {
            if (_enabled())
                Trace.WriteLine(exception);
        }

        public void Write(Exception exception, string message)
        {
            if (_enabled())
            {
                Trace.WriteLine(message);
                Trace.WriteLine(exception);
            }
        }
    }
}