namespace MassTransit.Util
{
    using System;
    using System.Threading;


    /// <summary>
    /// Thread safe timer that allows efficient restarts by rolling the due time further into the future.
    /// Will roll over once every 43~ days of continuous runtime without a restart.
    /// </summary>
    public class RollingTimer :
        IDisposable
    {
        readonly TimerCallback _callback;
        readonly object _lock = new object();
        readonly object _state;
        TimeSpan _timeout;
        Timer _timer;
        int _triggered;

        public RollingTimer(TimerCallback callback, TimeSpan timeout, object state = default)
        {
            void Callback(object obj)
            {
                Set();
                callback(obj);
            }

            _callback = Callback;
            _timeout = timeout;
            _state = state;
        }

        public bool Triggered => _triggered == 1;

        public void Dispose()
        {
            lock (_lock)
            {
                _timer?.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Creates a new timer and starts it.
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                _timer?.Dispose();
                StartInternal();
            }
        }

        /// <summary>
        /// Stops and disposes the existing timer.
        /// </summary>
        public void Stop()
        {
            Dispose();
        }

        /// <summary>
        /// Restarts the existing timer, creates and starts a new timer if it does not exist.
        /// </summary>
        public void Restart(TimeSpan? timeout = null)
        {
            lock (_lock)
            {
                if (timeout.HasValue)
                    _timeout = timeout.Value;

                if (_timer == null)
                    StartInternal();
                else
                {
                    Reset();
                    _timer.Change(_timeout, TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        void StartInternal()
        {
            Reset();
            _timer = new Timer(_callback, _state, _timeout, TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Sets the timer as triggered
        /// </summary>
        void Set()
        {
            Interlocked.CompareExchange(ref _triggered, 1, 0);
        }

        /// <summary>
        /// Resets the trigger status
        /// </summary>
        void Reset()
        {
            Interlocked.CompareExchange(ref _triggered, 0, 1);
        }
    }
}
