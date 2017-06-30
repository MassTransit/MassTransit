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
namespace MassTransit.Logging.Tracing
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;


    public class TraceLogger :
        ILogger
    {
        readonly TraceSource _defaultSource;
        readonly ConcurrentDictionary<string, TraceLog> _logs;
        readonly ConcurrentDictionary<string, TraceSource> _sources;
        TraceListener _listener;

        public TraceLogger()
        {
            _logs = new ConcurrentDictionary<string, TraceLog>();
            _sources = new ConcurrentDictionary<string, TraceSource>();

            _defaultSource = new TraceSource("Default", SourceLevels.Information);

            _listener = AddDefaultConsoleTraceListener(_defaultSource);

            Get("MassTransit");
        }

        public ILog Get(string name)
        {
            return _logs.GetOrAdd(name, CreateTraceLog);
        }

        public void Shutdown()
        {
            Trace.Flush();

            if (_listener != null)
            {
                Trace.Listeners.Remove(_listener);

                _listener.Dispose();
                _listener = null;
            }
        }

        static TraceListener AddDefaultConsoleTraceListener(TraceSource source)
        {
            var listener = new ConsoleTraceListener {Name = "MassTransit"};

            source.Listeners.Add(listener);

            return listener;
        }

        TraceLog CreateTraceLog(string name)
        {
            return new TraceLog(_sources.GetOrAdd(name, CreateTraceSource));
        }

        TraceSource CreateTraceSource(string name)
        {
            LogLevel logLevel = LogLevel.Info;
            SourceLevels sourceLevel = logLevel.SourceLevel;
            var source = new TraceSource(name, sourceLevel);
            if (IsSourceConfigured(source))
                return source;

            ConfigureTraceSource(source, name, sourceLevel);

            return source;
        }

        void ConfigureTraceSource(TraceSource source, string name, SourceLevels sourceLevel)
        {
            var defaultSource = _defaultSource;
            for (string parentName = ShortenName(name);
                !string.IsNullOrEmpty(parentName);
                parentName = ShortenName(parentName))
            {
                var parentSource = new TraceSource(parentName, sourceLevel);
                if (IsSourceConfigured(parentSource))
                {
                    defaultSource = parentSource;
                    break;
                }
            }

            source.Switch = defaultSource.Switch;
            source.Listeners.Clear();
            foreach (TraceListener listener in defaultSource.Listeners)
                source.Listeners.Add(listener);
        }

        static bool IsSourceConfigured(TraceSource source)
        {
            return source.Listeners.Count != 1
                || !(source.Listeners[0] is DefaultTraceListener)
                || source.Listeners[0].Name != "Default";
        }

        static string ShortenName(string name)
        {
            int length = name.LastIndexOf('.');

            return length != -1
                ? name.Substring(0, length)
                : null;
        }
    }
}