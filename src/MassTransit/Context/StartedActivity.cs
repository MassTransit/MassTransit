namespace MassTransit.Context
{
    using System.Diagnostics;
    using Logging;


    public readonly struct StartedActivity :
        StartedActivityContext
    {
        public readonly DiagnosticSource Source;
        public readonly Activity Activity;
        public readonly EnabledScope? Scope;

        public StartedActivity(DiagnosticSource source, Activity activity, EnabledScope? scope)
        {
            Source = source;
            Activity = activity;
            Scope = scope;
        }

        public StartedActivity AddTag(string key, string value)
        {
            Activity.AddTag(key, value);

            return this;
        }

        public StartedActivity AddBaggage(string key, string value)
        {
            Activity.AddBaggage(key, value);

            Scope?.Add(key, value);

            return this;
        }

        public void SetParentId(string parentId)
        {
            Activity.SetParentId(parentId);
        }

        public void Stop(object args = default)
        {
            Source.StopActivity(Activity, args);

            Scope?.Dispose();
        }
    }
}
