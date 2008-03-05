namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	public class ServiceBusAsyncResult :
		IServiceBusAsyncResult
	{
		private readonly AsyncCallback _callback;
		private readonly object _state;
		private readonly ManualResetEvent _waitHandle;
		private bool _completedSynchronously;
		private volatile bool _isComplete = false;
		private List<IMessage> _messages;

		/// <summary>
		/// Initializes a new <c ref="ServiceBusAsyncResult"/>
		/// </summary>
		public ServiceBusAsyncResult()
		{
			_waitHandle = new ManualResetEvent(false);
		}

		/// <summary>
		/// Initializes a new <c ref="ServiceBusAsyncResult"/>
		/// </summary>
		/// <param name="callback">The callback to invoke when the operation is complete</param>
		/// <param name="state">The caller state to identify the original request</param>
		public ServiceBusAsyncResult(AsyncCallback callback, object state)
			: this()
		{
			_callback = callback;
			_state = state;
		}

		#region IServiceBusAsyncResult Members

		public IList<IMessage> Messages
		{
			get
			{
				if (_messages == null)
					return new List<IMessage>();

				return _messages;
			}
		}

		///<summary>
		///Gets an indication whether the asynchronous operation has completed.
		///</summary>
		///
		///<returns>
		///true if the operation is complete; otherwise, false.
		///</returns>
		///<filterpriority>2</filterpriority>
		public bool IsCompleted
		{
			get { return _isComplete; }
		}

		///<summary>
		///Gets a <see cref="System.Threading.WaitHandle"></see> that is used to wait for an asynchronous operation to complete.
		///</summary>
		///
		///<returns>
		///A <see cref="System.Threading.WaitHandle"></see> that is used to wait for an asynchronous operation to complete.
		///</returns>
		///<filterpriority>2</filterpriority>
		public WaitHandle AsyncWaitHandle
		{
			get { return _waitHandle; }
		}

		///<summary>
		///Gets a user-defined object that qualifies or contains information about an asynchronous operation.
		///</summary>
		///
		///<returns>
		///A user-defined object that qualifies or contains information about an asynchronous operation.
		///</returns>
		///<filterpriority>2</filterpriority>
		public object AsyncState
		{
			get { return _state; }
		}

		///<summary>
		///Gets an indication of whether the asynchronous operation completed synchronously.
		///</summary>
		///
		///<returns>
		///true if the asynchronous operation completed synchronously; otherwise, false.
		///</returns>
		///<filterpriority>2</filterpriority>
		public bool CompletedSynchronously
		{
			get { return _completedSynchronously; }
		}

		#endregion

		/// <summary>
		/// Indicates that the operation has completed
		/// </summary>
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

		/// <summary>
		/// Indicates that the operation has completed
		/// </summary>
		public void Complete(params IMessage[] messages)
		{
			_messages = new List<IMessage>();
			_messages.AddRange(messages);

			Complete(false);
		}
	}
}