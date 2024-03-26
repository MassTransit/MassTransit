namespace MassTransit.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;


    public class TextWriterLoggerOptions
    {
        readonly List<string> _disabled;

        public TextWriterLoggerOptions()
        {
            _disabled = new List<string>();
        }

        public LogLevel LogLevel { get; set; }

        public TextWriterLoggerOptions Disable(string name)
        {
            _disabled.Add(name);

            return this;
        }

        public bool IsEnabled(string name)
        {
            return !_disabled.Any(x => name.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
