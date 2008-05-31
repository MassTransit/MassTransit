namespace MassTransit.ServiceBus.Threading
{
	using System;
	using System.Threading;

	public class ManagedThread : IDisposable
	{
		private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		private readonly Thread _thread;
		private readonly TimeSpan _threadExitWaitTime = TimeSpan.FromMinutes(1);

		public ManagedThread()
		{
			_thread = new Thread(RunThread);
		}

		protected ManualResetEvent Shutdown
		{
			get { return _shutdown; }
		}

		public void Dispose()
		{
			_shutdown.Set();
			_thread.Join(_threadExitWaitTime);
		}

		public void Start()
		{
			if (_thread.IsAlive)
				return;

			_shutdown.Reset();
			_thread.Start();
		}

		public void Start(object obj)
		{
			if (_thread.IsAlive)
				return;

			_shutdown.Reset();
			_thread.Start(obj);
		}

		public void Stop()
		{
			if (_thread.IsAlive)
			{
				_shutdown.Set();
				_thread.Join(_threadExitWaitTime);
			}
		}

		protected virtual void RunThread(object obj)
		{
			WaitHandle[] handles = new WaitHandle[] {_shutdown};

			int result;
			while ((result = WaitHandle.WaitAny(handles, TimeSpan.FromSeconds(5), true)) != 0)
			{
				if (result == WaitHandle.WaitTimeout)
					continue;
			}
		}
	}
}