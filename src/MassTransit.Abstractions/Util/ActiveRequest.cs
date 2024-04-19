namespace MassTransit.Util
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public struct ActiveRequest :
        IDisposable
    {
        readonly RequestRateAlgorithm _algorithm;
        readonly CancellationTokenRegistration _registration;
        readonly CancellationTokenSource _source;
        readonly TimeSpan _timeout;
        bool _completed;

        public readonly CancellationToken CancellationToken;
        public readonly int ResultLimit;

        public ActiveRequest(RequestRateAlgorithm algorithm, int resultLimit, CancellationToken cancellationToken, TimeSpan timeout)
        {
            _algorithm = algorithm;
            _timeout = timeout;
            _source = new CancellationTokenSource();
            _registration = cancellationToken.Register(Callback, this);

            CancellationToken = _source.Token;
            ResultLimit = resultLimit;

            _completed = false;
        }

        public Task Complete(int count, CancellationToken cancellationToken = default)
        {
            _completed = true;

            return _algorithm.EndRequest(count, ResultLimit, cancellationToken);
        }

        public void Dispose()
        {
            _registration.Dispose();
            _source.Dispose();

            if (_completed)
                return;

            _algorithm.CancelRequest(ResultLimit);
        }

        void Callback(object? obj)
        {
            _source.CancelAfter(_timeout);
        }
    }
}
