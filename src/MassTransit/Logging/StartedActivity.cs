#nullable enable
namespace MassTransit.Logging
{
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

        public void Stop()
        {
            Activity.Stop();
        }
    }
}
