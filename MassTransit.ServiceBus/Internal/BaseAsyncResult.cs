namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Threading;

	public class BaseAsyncResult :
		IAsyncRequest
	{
		private readonly ManualResetEvent _asyncWaitHandle;
		private readonly AsyncCallback _callback;
		private readonly object _state;
		private volatile bool _completedSynchronously;
		private volatile bool _isComplete = false;

		protected BaseAsyncResult()
		{
			_asyncWaitHandle = new ManualResetEvent(false);
		}

		protected BaseAsyncResult(AsyncCallback callback, object state)
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
			get { return _asyncWaitHandle; }
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

		public virtual void Complete(bool synchronously)
		{
			_isComplete = true;
			_completedSynchronously = synchronously;
			_asyncWaitHandle.Set();

			if (_callback != null)
				_callback(this);
		}

		public virtual void Cancel()
		{
		}
	}
}