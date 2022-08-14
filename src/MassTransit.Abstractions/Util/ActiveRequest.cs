namespace MassTransit.Util
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public struct ActiveRequest :
        IDisposable
    {
        public readonly int ResultLimit;
        readonly RequestRateAlgorithm _algorithm;
        bool _completed;

        public ActiveRequest(RequestRateAlgorithm algorithm, int resultLimit)
        {
            ResultLimit = resultLimit;
            _algorithm = algorithm;
            _completed = false;
        }

        public Task Complete(int count, CancellationToken cancellationToken = default)
        {
            _completed = true;

            return _algorithm.EndRequest(count, ResultLimit, cancellationToken);
        }

        public void Dispose()
        {
            if (_completed)
                return;

            _algorithm.CancelRequest(ResultLimit);
        }
    }
}
