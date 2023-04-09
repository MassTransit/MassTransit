#nullable enable
namespace MassTransit.Logging
{
    using Microsoft.Extensions.Logging;


    /// <summary>
    /// Used to provide access to logging and diagnostic services
    /// </summary>
    public interface ILogContext
    {
        ILogger Logger { get; }

        /// <summary>
        /// The log context for all message movement, sent, received, etc.
        /// </summary>
        ILogContext Messages { get; }

        EnabledLogger? Critical { get; }
        EnabledLogger? Debug { get; }
        EnabledLogger? Error { get; }
        EnabledLogger? Info { get; }
        EnabledLogger? Trace { get; }
        EnabledLogger? Warning { get; }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Logging.ILogger" />.</returns>
        ILogContext CreateLogContext(string categoryName);
    }
}
