namespace MassTransit.Context
{
    using Logging;
    using Microsoft.Extensions.Logging;


    /// <summary>
    /// Used to provide access to logging and diagnostic services
    /// </summary>
    public interface ILogContext
    {
        /// <summary>
        /// The log context for all message movement, sent, received, etc.
        /// </summary>
        ILogContext Messages { get; }

        /// <summary>
        /// If enabled, returns a valid source which can be used
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A valid source, or null</returns>
        EnabledDiagnosticSource? IfEnabled(string name);

        /// <summary>
        /// Creates a new ILogger instance using the full name of the given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        ILogContext<T> CreateLogContext<T>();

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Logging.ILogger" />.</returns>
        ILogContext CreateLogContext(string categoryName);

        EnabledLogger? IfEnabled(LogLevel level);
        EnabledLogger? Critical { get; }
        EnabledLogger? Debug { get; }
        EnabledLogger? Error { get; }
        EnabledLogger? Info { get; }
        EnabledLogger? Trace { get; }
        EnabledLogger? Warning { get; }

        /// <summary>
        /// Begin a scope for the logger
        /// </summary>
        /// <returns></returns>
        EnabledScope? BeginScope();
    }


    public interface ILogContext<T> :
        ILogContext
    {
    }
}
