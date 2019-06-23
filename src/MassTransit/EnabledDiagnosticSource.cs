namespace MassTransit
{
    using System.Diagnostics;


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

            return new StartedActivity(_source, _source.StartActivity(activity, args));
        }
    }
}
