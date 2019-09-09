namespace MassTransit.Logging
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;


    public readonly struct EnabledScope
    {
        readonly IDisposable _scope;
        readonly IDictionary<string, object> _state;

        public EnabledScope(ILogger logger)
        {
            _state = new Dictionary<string, object>();

            _scope = logger.BeginScope(_state);
        }

        public void Add(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == default)
                _state.Remove(key);
            else
                _state[key] = value;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
