namespace MassTransit.Logging
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Context;


    public readonly struct EnabledDiagnosticSource
    {
        readonly DiagnosticSource _source;
        readonly string _name;

        public EnabledDiagnosticSource(DiagnosticSource source, string name)
        {
            _source = source;
            _name = name;
        }

        public StartedActivity? StartActivity(IEnumerable<(string, string)> bags = null, IEnumerable<(string, string)> tags = null,
            string parentActivityId = null)
        {
            var activity = new Activity(_name);

            if (parentActivityId != null)
                activity.SetParentId(parentActivityId);

            if (bags != null)
            {
                foreach (var (key, value) in bags)
                    activity.AddBaggage(key, value);
            }

            if (tags != null)
            {
                foreach (var (key, value) in tags)
                    activity.AddTag(key, value);
            }

            var startActivity = _source.StartActivity(activity, null);

            EnabledScope? scope = LogContext.BeginScope();

            return new StartedActivity(_source, startActivity, scope);
        }
    }
}
