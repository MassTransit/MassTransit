#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.Diagnostics;


    public readonly struct StartedActivity
    {
        public readonly Activity Activity;

        public StartedActivity(Activity activity)
        {
            Activity = activity;
        }

        public void AddTag(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            Activity.AddTag(key, value);
        }

        public void Update<T>(SendContext<T> context)
            where T : class
        {
            if (context.BodyLength.HasValue)
                AddTag(DiagnosticHeaders.Messaging.BodyLength, context.BodyLength.Value.ToString());
        }

        public void RecordException(Exception exception, bool escaped)
        {
            var tags = new ActivityTagsCollection
            {
                { DiagnosticHeaders.Exceptions.Escaped, escaped },
                { DiagnosticHeaders.Exceptions.Message, exception.Message },
                { DiagnosticHeaders.Exceptions.Type, exception.GetType().Name },
                { DiagnosticHeaders.Exceptions.Stacktrace, exception.ToString() }
            };

            var activityEvent = new ActivityEvent(DiagnosticHeaders.Exceptions.EventName, DateTimeOffset.UtcNow, tags);

            Activity.AddEvent(activityEvent);
            Activity.SetStatus(ActivityStatusCode.Error);
        }

        public void Stop()
        {
            Activity.Stop();
        }
    }
}
