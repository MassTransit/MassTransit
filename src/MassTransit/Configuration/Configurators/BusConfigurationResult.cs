namespace MassTransit.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using GreenPipes;


    [Serializable, DebuggerDisplay("{DebuggerString()}")]
    public class BusConfigurationResult :
        ConfigurationResult
    {
        readonly IList<ValidationResult> _results;

        BusConfigurationResult(IEnumerable<ValidationResult> results)
        {
            _results = results.ToList();
        }

        public bool ContainsFailure
        {
            get { return _results.Any(x => x.Disposition == ValidationResultDisposition.Failure); }
        }

        public IEnumerable<ValidationResult> Results => _results;

        protected string DebuggerString()
        {
            string debuggerString = string.Join(", ", _results);

            return string.IsNullOrWhiteSpace(debuggerString)
                ? "No Obvious Problems says ConfigurationResult"
                : debuggerString;
        }

        public static ConfigurationResult CompileResults(IEnumerable<ValidationResult> results)
        {
            var result = new BusConfigurationResult(results);
            if (!result.ContainsFailure)
                return result;

            string message = "The service bus was not properly configured:" + Environment.NewLine +
                string.Join(Environment.NewLine, result.Results.Select(x => x.ToString()).ToArray());

            throw new ConfigurationException(result, message);
        }
    }
}
