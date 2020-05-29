namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Configurators;


    [Serializable]
    [DebuggerDisplay("{" + nameof(DebuggerString) + "()}")]
    public class StateMachineConfigurationResult :
        ConfigurationResult
    {
        readonly IList<ValidationResult> _results;

        StateMachineConfigurationResult(IEnumerable<ValidationResult> results)
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
            var debuggerString = string.Join(", ", _results);

            return string.IsNullOrWhiteSpace(debuggerString)
                ? "No Obvious Problems says ConfigurationResult"
                : debuggerString;
        }

        public static ConfigurationResult CompileResults(IEnumerable<ValidationResult> results)
        {
            var result = new StateMachineConfigurationResult(results);

            if (result.ContainsFailure)
            {
                var message = "The state machine was not properly configured:" +
                    Environment.NewLine +
                    string.Join(Environment.NewLine, result.Results.Select(x => x.ToString()).ToArray());

                throw new ConfigurationException(result, message);
            }

            return result;
        }
    }
}
