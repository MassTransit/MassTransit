namespace MassTransit.Util
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public struct ActiveRequest :
        IDisposable
    {
        readonly RequestRateAlgorithm _algorithm;
        bool _completed;

        public ActiveRequest(RequestRateAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _completed = false;
        }

        public Task Complete(int count, CancellationToken cancellationToken = default)
        {
            _completed = true;

            return _algorithm.EndRequest(count, cancellationToken);
        }

        public void Dispose()
        {
            if (_completed)
                return;

            _algorithm.CancelRequest();
        }
    }
}
