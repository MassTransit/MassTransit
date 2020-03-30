namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public class ExceptionWithVariables : Exception
    {
        public ExceptionWithVariables()
        {
        }

        public ExceptionWithVariables(IDictionary<string, object> variables, string message)
            : base(message)
        {
            Variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }

        public ExceptionWithVariables(IDictionary<string, object> variables, string message, Exception innerException)
            : base(message, innerException)
        {
            Variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }

        public ExceptionWithVariables(string message)
            : base(message)
        {
        }

        public ExceptionWithVariables(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ExceptionWithVariables(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public IDictionary<string, object> Variables { get; protected set; }
    }
}
