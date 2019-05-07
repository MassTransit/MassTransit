namespace MassTransit
{
    using Microsoft.Extensions.Logging;
    using Serilog.Debugging;
    using Serilog.Extensions.Logging;


    public class SerilogLoggerFactory : ILoggerFactory
    {
        readonly SerilogLoggerProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLoggerFactory"/> class.
        /// </summary>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="Serilog.Log"/> will be used.</param>
        /// <param name="dispose">When true, dispose <paramref name="logger"/> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose"/> is true, the <see cref="Serilog.Log.CloseAndFlush()"/> method will be
        /// called on the static <see cref="Serilog.Log"/> class instead.</param>
        public SerilogLoggerFactory(Serilog.ILogger logger = null, bool dispose = false)
        {
            _provider = new SerilogLoggerProvider(logger, dispose);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose()
        {
            _provider.Dispose();
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>
        /// The <see cref="T:Microsoft.Extensions.Logging.ILogger" />.
        /// </returns>
        public ILogger CreateLogger(string categoryName)
        {
            return _provider.CreateLogger(categoryName);
        }

        /// <summary>
        /// Adds an <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> to the logging system.
        /// </summary>
        /// <param name="provider">The <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" />.</param>
        public void AddProvider(ILoggerProvider provider)
        {
            SelfLog.WriteLine("Ignoring added logger provider {0}", provider);
        }
    }
}
