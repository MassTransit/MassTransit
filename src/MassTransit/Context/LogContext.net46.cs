namespace MassTransit.Context
{
    using System.Threading;


    public partial class LogContext
    {
        static readonly AsyncLocal<ILogContext> _current = new AsyncLocal<ILogContext>();

        /// <summary>
        /// Gets or sets the current operation (Activity) for the current thread.  This flows
        /// across async calls.
        /// </summary>
        public static ILogContext Current
        {
            get => _current.Value;
            set
            {
                if (ValidateSetCurrent(value))
                {
                    SetCurrent(value);
                }
            }
        }

        static void SetCurrent(ILogContext activity)
        {
            _current.Value = activity;
        }
    }
}
