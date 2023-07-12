#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.Diagnostics;
    using Util;


    public readonly struct StartedActivity
    {
        public readonly Activity Activity;

        public StartedActivity(Activity activity)
        {
            Activity = activity;
        }

        public void SetTag(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            Activity.SetTag(key, value);
        }

        public void Update<T>(SendContext<T> context)
            where T : class
        {
            if (context.BodyLength.HasValue)
                SetTag(DiagnosticHeaders.Messaging.BodyLength, context.BodyLength.Value.ToString());
        }

        public void AddExceptionEvent(Exception exception, bool escaped = true)
        {
            exception = exception.GetBaseException() ?? exception;

            var exceptionMessage = ExceptionUtil.GetMessage(exception);

            var tags = new ActivityTagsCollection
            {
                { DiagnosticHeaders.Exceptions.Escaped, escaped },
                { DiagnosticHeaders.Exceptions.Message, exceptionMessage },
                { DiagnosticHeaders.Exceptions.Type, TypeCache.GetShortName(exception.GetType()) },
                { DiagnosticHeaders.Exceptions.Stacktrace, ExceptionUtil.GetStackTrace(exception) }
            };

            var activityEvent = new ActivityEvent(DiagnosticHeaders.Exceptions.EventName, DateTimeOffset.UtcNow, tags);

            Activity.AddEvent(activityEvent);
            Activity.SetStatus(ActivityStatusCode.Error, exceptionMessage);
        }

        public void Stop()
        {
            if (Activity.Status == ActivityStatusCode.Unset)
                Activity.SetStatus(ActivityStatusCode.Ok);

            Activity.Dispose();
        }
    }
}
