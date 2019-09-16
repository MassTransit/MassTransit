namespace MassTransit.Context
{
    using System;
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

        public void AddTag(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            Activity.AddTag(key, value);
        }

        public void AddTag(string key, Guid? value)
        {
            if (value.HasValue)
                Activity.AddTag(key, value.Value.ToString("D"));
        }

        public void AddTag(string key, Uri value)
        {
            if (value != null)
                Activity.AddTag(key, value.ToString());
        }

        public void AddBaggage(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            Activity.AddBaggage(key, value);

            Scope?.Add(key, value);
        }

        public void AddBaggage(string key, Guid? value)
        {
            if (value.HasValue)
                Activity.AddBaggage(key, value.Value.ToString("D"));
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
