#nullable enable
namespace MassTransit.Logging
{
    using System;


    public readonly struct StartedInstrument
    {
        readonly Action<Exception> _onFault;
        readonly Action? _onStop;

        public StartedInstrument(Action<Exception> onFault, Action? onStop = default)
        {
            _onFault = onFault;
            _onStop = onStop;
        }

        public void AddException(Exception exception)
        {
            _onFault(exception);
        }

        public void Stop()
        {
            _onStop?.Invoke();
        }
    }
}
