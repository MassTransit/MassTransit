// Copyright 2010 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.TestFramework
{
	using System;
	using System.Threading;
	using Magnum;
	using Magnum.Extensions;

	/// <summary>
	/// A future object that supports both callbacks and asynchronous waits once a future value becomes available.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Future<T> :
		IAsyncResult
	{
		readonly AsyncCallback _callback;
		readonly ManualResetEvent _event;
		readonly object _state;
		volatile bool _completed;

		public Future()
			: this(NullCallback, 0)
		{
		}

		public Future(AsyncCallback callback, object state)
		{
			_callback = callback;
			_state = state;

			_event = new ManualResetEvent(false);
		}

		public T Value { get; private set; }

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return _event; }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public void Complete(T message)
		{
			if (_completed)
			{
				throw new InvalidOperationException("A Future cannot be completed twice, value = {0}, passed = {1}".FormatWith(Value, message));
			}

			Value = message;

			_completed = true;

			_event.Set();

			_callback(this);
		}

		public bool WaitUntilCompleted(TimeSpan timeout)
		{
			return _event.WaitOne(timeout);
		}

		public bool WaitUntilCompleted(int timeout)
		{
			return _event.WaitOne(timeout);
		}

		~Future()
		{
			_event.Close();
		}

		static void NullCallback(object state)
		{
		}
	}
}