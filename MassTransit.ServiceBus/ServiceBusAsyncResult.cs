/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.ServiceBus
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