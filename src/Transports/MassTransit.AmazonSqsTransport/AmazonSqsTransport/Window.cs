using System.Threading.Tasks;

namespace MassTransit.AmazonSqsTransport
{
    public class Window
    {
        private volatile bool _isOpened = true;

        private object _isOpenedLock = new object();

        private TaskCompletionSource<bool> _nextWindowEvent = new TaskCompletionSource<bool>();     
        private Task _waitForOpen => _nextWindowEvent.Task;

        private readonly int _maxRequests;
        private volatile int _activeRequests = 0;

        public int RequestsToReceive
        {
            get
            {
                lock (_isOpenedLock)
                {
                    return _maxRequests - _activeRequests;
                }
            }
        }

        public Window(int maxRequests)
        {
            _maxRequests = maxRequests;
            _nextWindowEvent.SetResult(true);
        }

        /// <summary>
        /// Open window when any request finishes
        /// </summary>
        public void Open()
        {
            lock (_isOpenedLock)
            {
                _activeRequests -= 1;

                if (_isOpened)
                    return;

                if(_activeRequests < _maxRequests)
                {
                    _isOpened = true;
                    _nextWindowEvent.SetResult(true);
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
            lock (_isOpenedLock)
            {
                _activeRequests += requestsReceived;

                if (!_isOpened)
                    return;

                if(_activeRequests == _maxRequests)
                {
                    _isOpened = false;
                    _nextWindowEvent = new TaskCompletionSource<bool>();
                }
            }
        }
    }
}
