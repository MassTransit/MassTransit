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
namespace MassTransit.Logging.Tracing
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class LogLevel
    {
        public static readonly LogLevel All = new LogLevel("All", 6, SourceLevels.All, TraceEventType.Verbose);
        public static readonly LogLevel Debug = new LogLevel("Debug", 5, SourceLevels.Verbose, TraceEventType.Verbose);
        public static readonly LogLevel Error = new LogLevel("Error", 2, SourceLevels.Error, TraceEventType.Error);
        public static readonly LogLevel Fatal = new LogLevel("Fatal", 1, SourceLevels.Critical, TraceEventType.Critical);

        public static readonly LogLevel Info = new LogLevel("Info", 4, SourceLevels.Information,
            TraceEventType.Information);

        public static readonly LogLevel None = new LogLevel("None", 0, SourceLevels.Off, TraceEventType.Critical);
        public static readonly LogLevel Warn = new LogLevel("Warn", 3, SourceLevels.Warning, TraceEventType.Warning);

        readonly int _index;
        readonly string _name;
        readonly SourceLevels _sourceLevel;
        readonly TraceEventType _traceEventType;

        LogLevel(string name, int index, SourceLevels sourceLevel, TraceEventType traceEventType)
        {
            _name = name;
            _index = index;
            _sourceLevel = sourceLevel;
            _traceEventType = traceEventType;
        }

        public static bool operator >(LogLevel left, LogLevel right)
        {
            return right != null && (left != null && left._index > right._index);
        }

        public static bool operator <(LogLevel left, LogLevel right)
        {
            return right != null && (left != null && left._index < right._index);
        }

        public static bool operator >=(LogLevel left, LogLevel right)
        {
            return right != null && (left != null && left._index >= right._index);
        }

        public static bool operator <=(LogLevel left, LogLevel right)
        {
            return right != null && (left != null && left._index <= right._index);
        }

        public static IEnumerable<LogLevel> Values
        {
            get
            {
                yield return All;
                yield return Debug;
                yield return Info;
                yield return Warn;
                yield return Error;
                yield return Fatal;
                yield return None;
            }
        }

        public TraceEventType TraceEventType
        {
            get { return _traceEventType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public SourceLevels SourceLevel
        {
            get { return _sourceLevel; }
        }

        public override string ToString()
        {
            return _name;
        }

        public static LogLevel FromSourceLevels(SourceLevels level)
        {
            switch (level)
            {
                case SourceLevels.Information:
                    return Info;
                case SourceLevels.Verbose:
                    return Debug;
                case ~SourceLevels.Off:
                    return Debug;
                case SourceLevels.Critical:
                    return Fatal;
                case SourceLevels.Error:
                    return Error;
                case SourceLevels.Warning:
                    return Warn;
                default:
                    return None;
            }
        }
    }
}