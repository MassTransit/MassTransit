namespace MassTransit.Scoping
{
    using System;


    /// <summary>
    /// Captures the <see cref="ConsumeContext"/> for the current message as a scoped provider, so that it can be resolved
    /// by components at runtime (since MS DI doesn't support runtime configuration of scopes)
    /// </summary>
    public class ScopedConsumeContextProvider
    {
        public const string Any = "*";

        ConsumeContext _context;
        ScopedConsumeContext _marker;
        string _name;

        public void SetContext(string name, ConsumeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            lock (this)
            {
                if (_context == null)
                {
                    _context = context;
                    _name = name;
                    _marker = new ScopedConsumeContext();

                    context.GetOrAddPayload(() => _marker);
                }
                else if (ReferenceEquals(_context, context) && _name == name)
                {
                }
                else if (!context.TryGetPayload<ScopedConsumeContext>(out _))
                {
                    throw new InvalidOperationException("The ConsumeContext was already set.");
                }
            }
        }

        public ConsumeContext GetContext(string name)
        {
            return name == Any || name == _name ? _context : null;
        }


        class ScopedConsumeContext
        {
        }
    }
}
