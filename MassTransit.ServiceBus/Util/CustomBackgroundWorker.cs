using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace MassTransit.ServiceBus.Util
{
	///<summary>
	///</summary>
	public class CustomBackgroundWorker :
		BackgroundWorker
	{
		#region WaitHandleIndex enum

		///<summary>
		///</summary>
		public enum WaitHandleIndex
		{
			/// <summary>
			/// The exit event
			/// </summary>
			Exit = 0,
			/// <summary>
			/// The trigger semaphore
			/// </summary>
			Trigger = 1,
		}

		#endregion

		private readonly ManualResetEvent _deadEvent = new ManualResetEvent(true);
		private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);

		private readonly Semaphore _triggerSemaphore = new Semaphore(0, int.MaxValue);

		private readonly List<WaitHandle> _waitHandles = new List<WaitHandle>();

		protected CustomBackgroundWorker()
		{
			_waitHandles.Add(_exitEvent);
			_waitHandles.Add(_triggerSemaphore);
		}

		public CustomBackgroundWorker(DoWorkEventHandler handler)
			: this()
		{
			DoWork += handler;
			RunWorkerCompleted += CustomBackgroundWorker_RunWorkerCompleted;
		}

		public CustomBackgroundWorker(DoWorkEventHandler handler, bool autoStart)
			: this(handler)
		{
			if (autoStart)
				RunWorkerAsync();
		}

		public CustomBackgroundWorker(DoWorkEventHandler handler, object argument, bool autoStart)
			: this(handler)
		{
			if (autoStart)
				RunWorkerAsync(argument);
		}

		/// <summary>
		/// Set when the worker should be terminated
		/// </summary>
		public ManualResetEvent ExitEvent
		{
			get { return _exitEvent; }
		}

		/// <summary>
		/// Set when the worker has exited
		/// </summary>
		public ManualResetEvent DeadEvent
		{
			get { return _deadEvent; }
		}

		/// <summary>
		/// The semaphore to fire events for processing by the worker
		/// </summary>
		public Semaphore TriggerSemaphore
		{
			get { return _triggerSemaphore; }
		}

		/// <summary>
		/// The wait handles for the background worker
		/// </summary>
		public List<WaitHandle> WaitHandles
		{
			get { return _waitHandles; }
		}

		private void CustomBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (sender is CustomBackgroundWorker)
			{
				CustomBackgroundWorker worker = sender as CustomBackgroundWorker;
				worker.DeadEvent.Set();
			}
		}


		///<summary>
		///</summary>
		///<returns></returns>
		public int Trigger()
		{
			return _triggerSemaphore.Release();
		}

		///<summary>
		///</summary>
		///<param name="count"></param>
		///<returns></returns>
		public int Trigger(int count)
		{
			return _triggerSemaphore.Release(count);
		}

		///<summary>
		///</summary>
		public void Start()
		{
			if (_deadEvent.WaitOne(0, false))
			{
				_exitEvent.Reset();
				_deadEvent.Reset();

				RunWorkerAsync();
			}
		}

		///<summary>
		///</summary>
		///<param name="argument"></param>
		public void Start(object argument)
		{
			if (_deadEvent.WaitOne(0, false))
			{
				_exitEvent.Reset();
				_deadEvent.Reset();

				RunWorkerAsync(argument);
			}
		}

		/// <summary>
		/// Stop the worker thread from processing
		/// </summary>
		public void Stop()
		{
			_exitEvent.Set();
		}

		/// <summary>
		/// Stop the worker thread from processing, waiting until the timeout has elapsed if the thread does not signal death
		/// </summary>
		/// <param name="timeout"></param>
		public void Stop(TimeSpan timeout)
		{
			_exitEvent.Set();

			_deadEvent.WaitOne(timeout, false);
		}
	}
}