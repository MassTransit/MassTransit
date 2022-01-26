#nullable enable
namespace MassTransit.Logging
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Logging;


    public class TextWriterLoggerFactory :
        ILoggerFactory
    {
        readonly bool _enabled;

        public TextWriterLoggerFactory(TextWriter textWriter, bool enabled)
        {
            Writer = textWriter;
            _enabled = enabled;
        }

        public TextWriterLoggerFactory(bool enabled)
        {
            _enabled = enabled;

            Writer = Console.Out;
        }

        public TextWriter Writer { get; }

        public ILogger CreateLogger(string name)
        {
            return new TextWriterLogger(this, _enabled);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
