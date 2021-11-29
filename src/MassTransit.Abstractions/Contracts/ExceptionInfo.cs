namespace MassTransit
{
    using System.Collections.Generic;


    /// <summary>
    /// An exception information that is serializable
    /// </summary>
    public interface ExceptionInfo
    {
        /// <summary>
        /// The type name of the exception
        /// </summary>
        string ExceptionType { get; }

        /// <summary>
        /// The inner exception if present (also converted to ExceptionInfo)
        /// </summary>
        ExceptionInfo InnerException { get; }

        /// <summary>
        /// The stack trace of the exception site
        /// </summary>
        string StackTrace { get; }

        /// <summary>
        /// The exception message
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The exception source
        /// </summary>
        string Source { get; }

        IDictionary<string, object> Data { get; }
    }
}
