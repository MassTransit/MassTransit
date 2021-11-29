namespace MassTransit.DependencyInjection
{
    using System;
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

        public ConsumeContext GetContext()
        {
            return _context;
        }


        class ScopedConsumeContext
        {
        }
    }
}
