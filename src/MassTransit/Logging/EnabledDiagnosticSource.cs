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

        public StartedActivity? StartActivity(object args = null)
        {
            Activity activity = new Activity(_name);

            var startActivity = _source.StartActivity(activity, args);

            var scope = LogContext.BeginScope();

            return new StartedActivity(_source, startActivity, scope);
        }
    }


    /// <summary>
    /// This is an idea that might eventually be able to support passing value tuples and having them map to an argument type array
    /// </summary>
    readonly struct StartActivityArgument
    {
        public readonly string Key;
        public readonly object Value;

        public StartActivityArgument(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public static implicit operator StartActivityArgument((string, object) input)
        {
            return new StartActivityArgument();
        }

        public static implicit operator KeyValuePair<string, object>(StartActivityArgument argument)
        {
            return new KeyValuePair<string, object>(argument.Key, argument.Value);
        }
    }
}
