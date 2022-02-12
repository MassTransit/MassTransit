namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;


    public static class ExceptionUtil
    {
        static readonly Regex _cleanup;

        static ExceptionUtil()
        {
            const string awaiter = @"at System\.Runtime\.CompilerServices\.TaskAwaiter.*";
            const string exception = @"at System\.Runtime\.ExceptionServices\.ExceptionDispatchInfo.*";

            _cleanup = new Regex(@"\n\s+(" + string.Join("|", awaiter, exception) + ")+",
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

        public static string GetStackTrace(Exception? exception)
        {
            if (string.IsNullOrWhiteSpace(exception?.StackTrace))
                return "";

            return _cleanup.Replace(exception!.StackTrace, "");
        }

        public static IDictionary<string, object> GetExceptionHeaderDictionary(Exception exception)
        {
            exception = exception.GetBaseException() ?? exception;

            var exceptionMessage = GetMessage(exception);

            return new Dictionary<string, object>
            {
                { MessageHeaders.Reason, "fault" },
                { MessageHeaders.FaultExceptionType, TypeCache.GetShortName(exception.GetType()) },
                { MessageHeaders.FaultMessage, exceptionMessage },
                { MessageHeaders.FaultStackTrace, GetStackTrace(exception) }
            };
        }
    }
}
