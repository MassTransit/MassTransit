namespace MassTransit.Scoping
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Captures the <see cref="ConsumeContext"/> for the current message as a scoped provider, so that it can be resolved
    /// by components at runtime (since MS DI doesn't support runtime configuration of scopes)
    /// </summary>
    public class ScopedConsumeContextProvider
    {
        readonly IDictionary<string, ConsumeContext> _contexts = new Dictionary<string, ConsumeContext>();

        public void SetContext(string name, ConsumeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            lock (_contexts)
            {
                if (!_contexts.TryGetValue(name, out var current))
                {
                    _contexts[name] = context;
                    context.GetOrAddPayload(() => new ScopedConsumeContext());
                }
                else if (ReferenceEquals(current, context))
                {
                }
                else if (!context.TryGetPayload<ScopedConsumeContext>(out _))
                {
                    throw new InvalidOperationException("The ConsumeContext was already set.");
                }
            }
        }

        public ConsumeContext GetContext(string name)
        {
            lock (_contexts)
            {
                if (!_contexts.TryGetValue(name, out var context))
                    throw new ArgumentException(nameof(name), $"The ConsumeContext ({name}) not found");
                return context;
            }
        }


        class ScopedConsumeContext
        {
        }
    }
}
