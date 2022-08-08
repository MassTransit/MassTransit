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
        ScopedConsumeContext _marker;

        public bool HasContext => _context != null && !(_context is MissingConsumeContext);

        public void SetContext(ConsumeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            lock (this)
            {
                if (_context == null)
                {
                    _context = context;
                    _marker = new ScopedConsumeContext();

                    context.GetOrAddPayload(() => _marker);
                }
                else if (ReferenceEquals(_context, context))
                {
                }
                else if (!context.TryGetPayload<ScopedConsumeContext>(out _))
                    throw new InvalidOperationException("The ConsumeContext was already set.");
            }
        }

        public IDisposable PushContext(ConsumeContext context)
        {
            if (_context == null)
                throw new InvalidOperationException("Unable to push context when a context does not already exist");
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (!context.TryGetPayload<ScopedConsumeContext>(out _))
                throw new ArgumentException("The consume context is not scoped", nameof(context));

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


        class ScopedConsumeContext
        {
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
