using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.AmazonSqsTransport
{
    public class Window
    {
        private volatile bool _isOpened = true;

        private object locker = new object();

        private TaskCompletionSource<bool> _thisWindowEvent = new TaskCompletionSource<bool>();
        private TaskCompletionSource<bool> _nextWindowEvent = new TaskCompletionSource<bool>();
        
        private Task _waitForOpen => _thisWindowEvent.Task;

        private readonly int _maxRequests;
        private volatile int _activeRequests = 0;

        private readonly CancellationToken _cancellationToken;

        public int RequestsToReceive
        {
            get
            {
                lock (locker)
                {
                    return _maxRequests - _activeRequests;
                }
            }
        }

        public Window(int maxRequests, CancellationToken cancellationToken)
        {
            _maxRequests = maxRequests;
            _thisWindowEvent.SetResult(true);
            _cancellationToken = cancellationToken;
            _cancellationToken.Register(() => Dispose());
        }

        /// <summary>
        /// Open window when any request finishes
        /// </summary>
        public void Open()
        {
            lock (locker)
            {
                _activeRequests -= 1;

                if (_isOpened)
                    return;

                if(_activeRequests < _maxRequests)
                {
                    _isOpened = true;
                    _thisWindowEvent.SetResult(true);
                }
            }
        }

        /// <summary>
        /// Wait if max amount of active requests is reached. 
        /// </summary>
        /// <returns></returns>
        public Task WaitForOpen()
        {
            return _waitForOpen;
        }

        /// <summary>
        /// Close window after messages are received from queue in order not to poll for new messages
        /// </summary>
        /// <param name="requestsReceived"></param>
        public void Close(int requestsReceived)
        {
            lock (locker)
            {
                _activeRequests += requestsReceived;

                if (!_isOpened)
                    return;

                if(_activeRequests == _maxRequests)
                {
                    _isOpened = false;
                    _nextWindowEvent = new TaskCompletionSource<bool>();
                    _thisWindowEvent = _nextWindowEvent;
                }
            }
        }

        public void Dispose()
        {
            _thisWindowEvent.TrySetCanceled(_cancellationToken);
        }
    }
}
