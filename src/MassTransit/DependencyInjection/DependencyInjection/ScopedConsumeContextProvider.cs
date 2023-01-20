namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;
    using Context;


    /// <summary>
    /// Captures the <see cref="ConsumeContext" /> for the current message as a scoped provider, so that it can be resolved
    /// by components at runtime (since MS DI doesn't support runtime configuration of scopes)
    /// </summary>
    public class ScopedConsumeContextProvider
    {
        ConsumeContext _context;

        public bool HasContext => _context != null && !(_context is MissingConsumeContext);

        public IDisposable PushContext(ConsumeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            lock (this)
            {
                var originalContext = _context;

                _context = context;

                return new PushedContext(this, context, originalContext);
            }
        }

        void PopContext(ConsumeContext context, ConsumeContext originalContext)
        {
            Interlocked.CompareExchange(ref _context, originalContext, context);
        }

        public ConsumeContext GetContext()
        {
            return _context;
        }


        class PushedContext :
            IDisposable
        {
            readonly ConsumeContext _context;
            readonly ConsumeContext _originalContext;
            readonly ScopedConsumeContextProvider _provider;

            public PushedContext(ScopedConsumeContextProvider provider, ConsumeContext context, ConsumeContext originalContext)
            {
                _provider = provider;
                _context = context;
                _originalContext = originalContext;
            }

            public void Dispose()
            {
                _provider.PopContext(_context, _originalContext);
            }
        }
    }
}
