namespace MassTransit.Context
{
    using System;
    using GreenPipes;
    using Microsoft.Extensions.Logging;


    public class BaseLogContext :
        BasePipeContext,
        ILogContext
    {
        readonly ILogContext _context;

        public BaseLogContext(ILogContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        public ILogContext Messages => _context.Messages;

        public EnabledDiagnosticSource? IfEnabled(string name)
        {
            return _context.IfEnabled(name);
        }

        public ILogContext<T> CreateLogContext<T>()
        {
            return _context.CreateLogContext<T>();
        }

        public ILogContext CreateLogContext(string categoryName)
        {
            return _context.CreateLogContext(categoryName);
        }

        public EnabledLogger? IfEnabled(LogLevel level)
        {
            return _context.IfEnabled(level);
        }

        public EnabledLogger? Critical
        {
            get { return _context.Critical; }
        }

        public EnabledLogger? Debug
        {
            get { return _context.Debug; }
        }

        public EnabledLogger? Error
        {
            get { return _context.Error; }
        }

        public EnabledLogger? Info
        {
            get { return _context.Info; }
        }

        public EnabledLogger? Trace
        {
            get { return _context.Trace; }
        }

        public EnabledLogger? Warning
        {
            get { return _context.Warning; }
        }
    }
}
