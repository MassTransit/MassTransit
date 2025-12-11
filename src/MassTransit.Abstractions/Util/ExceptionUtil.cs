namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Transports;


    public static class ExceptionUtil
    {
        static readonly Regex _trim;
        static readonly Regex _cleanup;

        static ExceptionUtil()
        {
            const string awaiter = @"at System\.Runtime\.CompilerServices\.TaskAwaiter.*";
            const string exception = @"at System\.Runtime\.ExceptionServices\.ExceptionDispatchInfo.*";
            const string dependency = @"at MassTransit\.DependencyInjection.*";

            _cleanup = new Regex(@"\n\s+(" + string.Join("|", awaiter, exception, dependency) + ")+", RegexOptions.Multiline | RegexOptions.Compiled);
            _trim = new Regex(@"in\s.*MassTransit.*\.cs.*$", RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public static string GetMessage(Exception exception)
        {
            try
            {
                var exceptionMessage = exception.Message ?? $"An exception of type {exception.GetType()} was thrown but the message was null.";

                if (exceptionMessage.Length > 2048)
                    exceptionMessage = exceptionMessage.Substring(0, 2048);

                return exceptionMessage;
            }
            catch
            {
                return $"An exception of type {exception.GetType()} was thrown but the Message property threw an exception.";
            }
        }

        public static string GetStackTrace(Exception? exception)
        {
            var stackTrace = exception?.StackTrace;
            if (string.IsNullOrWhiteSpace(stackTrace))
                return "";

            stackTrace = _cleanup.Replace(stackTrace, "");
            stackTrace = _trim.Replace(stackTrace, "");

            if (stackTrace.Length > 2048)
                stackTrace = stackTrace.Substring(0, 2048);

            return stackTrace;
        }

        [Obsolete("This method is obsolete and will be removed in a future version.")]
        public static IDictionary<string, object> GetExceptionHeaderDictionary(Exception exception)
        {
            (Dictionary<string, object>? dictionary, var message) = GetExceptionHeaderDetail(exception);

            return dictionary;
        }

        [Obsolete("This method is obsolete and will be removed in a future version.")]
        public static (Dictionary<string, object>, string) GetExceptionHeaderDetail(Exception exception)
        {
            exception = exception.GetBaseException() ?? exception;

            var exceptionMessage = GetMessage(exception);

            return (new Dictionary<string, object>
            {
                { MessageHeaders.Reason, "fault" },
                { MessageHeaders.FaultExceptionType, TypeCache.GetShortName(exception.GetType()) },
                { MessageHeaders.FaultMessage, exceptionMessage },
                { MessageHeaders.FaultStackTrace, GetStackTrace(exception) }
            }, exceptionMessage);
        }

        public static (Dictionary<string, object>, string) GetExceptionHeaderDetail(Exception exception, ITransportSetHeaderAdapter<object> adapter)
        {
            exception = exception.GetBaseException() ?? exception;

            var exceptionMessage = GetMessage(exception);

            var dictionary = new Dictionary<string, object>();

            adapter.Set(dictionary, MessageHeaders.Reason, "fault");
            adapter.Set(dictionary, MessageHeaders.FaultExceptionType, TypeCache.GetShortName(exception.GetType()));
            adapter.Set(dictionary, MessageHeaders.FaultMessage, exceptionMessage);
            adapter.Set(dictionary, MessageHeaders.FaultStackTrace, GetStackTrace(exception));

            return (dictionary, exceptionMessage);
        }
    }
}
