namespace MassTransit.Introspection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Util;


    public class ScopeProbeContext :
        ProbeContext
    {
        readonly CancellationToken _cancellationToken;
        readonly IDictionary<string, object> _variables;

        protected ScopeProbeContext(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _variables = new Dictionary<string, object>();
        }

        CancellationToken ProbeContext.CancellationToken => _cancellationToken;

        public void Add(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(value))
                _variables.Remove(key);
            else
                _variables[key] = value;
        }

        public void Add(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null || value is string s && string.IsNullOrEmpty(s))
                _variables.Remove(key);
            else
                _variables[key] = value;
        }

        public void Set(object values)
        {
            if (values != null)
                SetVariablesFromDictionary(ConvertObject.ToDictionary(values));
        }

        public void Set(IEnumerable<KeyValuePair<string, object>> values)
        {
            SetVariablesFromDictionary(values);
        }

        public ProbeContext CreateScope(string key)
        {
            var scope = new ScopeProbeContext(_cancellationToken);

            IList<ScopeProbeContext> list;

            if (_variables.TryGetValue(key, out var value))
            {
                list = value as IList<ScopeProbeContext>;
                if (list == null)
                    throw new InvalidOperationException("The key already exists and is not a scope collection: " + key);
            }
            else
            {
                list = new List<ScopeProbeContext>();
                _variables[key] = list;
            }

            list.Add(scope);
            return scope;
        }

        protected IDictionary<string, object> Build()
        {
            return _variables.ToDictionary(x => x.Key, item =>
            {
                if (item.Value is IList<ScopeProbeContext> list)
                {
                    if (list.Count == 1)
                        return list[0].Build();

                    return list.Select(x => x.Build()).ToArray();
                }

                return item.Value;
            });
        }

        void SetVariablesFromDictionary(IEnumerable<KeyValuePair<string, object>> values)
        {
            foreach (KeyValuePair<string, object> value in values)
            {
                if (value.Value == null || value.Value is string s && string.IsNullOrEmpty(s))
                    _variables.Remove(value.Key);
                else
                    _variables[value.Key] = value.Value;
            }
        }
    }
}
