namespace MassTransit.TestFramework.Logging
{
    using System;
    using System.Diagnostics;
    using MassTransit.Logging;


    public class DiagnosticListenerObserver :
        IObserver<DiagnosticListener>
    {
        IDisposable _handle;

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == LogCategoryName.MassTransit)
            {
                //_handle?.Dispose();

                _handle = value.Subscribe(new TestOutputListenerObserver());
            }
        }
    }
}
