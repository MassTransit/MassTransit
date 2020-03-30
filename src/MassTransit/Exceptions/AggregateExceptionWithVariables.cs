namespace MassTransit
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class AggregateExceptionWithVariables : AggregateException
    {
        public AggregateExceptionWithVariables(Exception exception, IDictionary<string, object> variables)
            : base(exception)
        {
            Variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }

        public IDictionary<string, object> Variables { get; protected set; }
    }
}
