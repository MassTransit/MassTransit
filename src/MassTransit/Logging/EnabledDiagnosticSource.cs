namespace MassTransit.Logging
{
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

        public StartedActivity? StartActivity(object args = null)
        {
            Activity activity = new Activity(_name);

            var startActivity = _source.StartActivity(activity, args);

            var scope = LogContext.BeginScope();

            return new StartedActivity(_source, startActivity, scope);
        }
    }
}
