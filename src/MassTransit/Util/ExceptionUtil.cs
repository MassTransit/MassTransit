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
namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Metadata;


    public static class ExceptionUtil
    {
        static readonly Regex _cleanup;

        static ExceptionUtil()
        {
            _cleanup = new Regex(
                @"--- End of stack trace.* ---.*\n\s+(at System\.Runtime\.CompilerServices\.TaskAwaiter.*\s*|at System\.Runtime\.ExceptionServices\.ExceptionDispatchInfo.*\s*)+",
                RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public static string GetMessage(Exception exception)
        {
            try
            {
                return exception.Message ?? $"An exception of type {exception.GetType()} was thrown but the message was null.";
            }
            catch
            {
                return $"An exception of type {exception.GetType()} was thrown but the Message property threw an exception.";
            }
        }

        public static string GetStackTrace(Exception exception)
        {
            if (string.IsNullOrWhiteSpace(exception?.StackTrace))
            {
                return "";
            }

            return _cleanup.Replace(exception.StackTrace, "");
        }

        public static IDictionary<string, object> GetExceptionHeaderDictionary(Exception exception)
        {
            exception = exception.GetBaseException() ?? exception;

            var exceptionMessage = GetMessage(exception);

            return new Dictionary<string, object>
            {
                {MessageHeaders.Reason, "fault"},
                {MessageHeaders.FaultExceptionType, TypeMetadataCache.GetShortName(exception.GetType())},
                {MessageHeaders.FaultMessage, exceptionMessage},
                {MessageHeaders.FaultStackTrace, ExceptionUtil.GetStackTrace(exception)}
            };
        }
    }
}
