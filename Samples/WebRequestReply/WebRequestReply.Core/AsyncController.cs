namespace WebRequestReply.Core
{
	using System;
	using System.Threading;

	public class AsyncController : IAsyncResult
	{
		private readonly AsyncCallback _callback;
		private readonly object _state;
		private readonly ManualResetEvent _waitHandle;
		private bool _completedSynchronously;
		private volatile bool _isComplete = false;

		public AsyncController()
		{
			_waitHandle = new ManualResetEvent(false);
		}

		public AsyncController(AsyncCallback callback, object state)
			: this()
		{
			_callback = callback;
			_state = state;
		}

		public bool IsCompleted
		{
			get { return _isComplete; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return _waitHandle; }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return _completedSynchronously; }
		}

		public void Complete()
		{
			Complete(false);
		}

		public void Complete(bool synchronously)
		{
			_isComplete = true;
			_completedSynchronously = synchronously;
			_waitHandle.Set();

			if (_callback != null)
				_callback(this);
		}
	}
}