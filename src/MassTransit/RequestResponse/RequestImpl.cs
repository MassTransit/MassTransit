// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RequestResponse
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Exceptions;
	using Magnum.Extensions;
	using log4net;

	public class RequestImpl<TRequest, TKey> :
		IRequest<TRequest, TKey>
		where TRequest : class, CorrelatedBy<TKey>
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (RequestImpl<TRequest, TKey>));
		readonly IList<AsyncCallback> _completionCallbacks;
		readonly object _lock = new object();
		readonly TRequest _message;

		ManualResetEvent _complete;
		bool _completed;
		Exception _exception;
		object _state;
		TimeSpan _timeout = TimeSpan.MaxValue;
		Action _timeoutCallback;
		RegisteredWaitHandle _waitHandle;

		public RequestImpl(TRequest message)
		{
			_message = message;
			_completionCallbacks = new List<AsyncCallback>();
			_timeoutCallback = DefaultTimeoutCallback;
		}

		ManualResetEvent CompleteEvent
		{
			get
			{
				lock (_lock)
				{
					if (_complete == null)
						_complete = new ManualResetEvent(false);
				}

				return _complete;
			}
		}

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return CompleteEvent; }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public bool Wait(TimeSpan timeout)
		{
			_timeout = timeout;

			return Wait();
		}

		public bool Wait()
		{
			bool alreadyCompleted;
			lock (_lock)
				alreadyCompleted = _completed;

			bool result = alreadyCompleted || CompleteEvent.WaitOne(_timeout, true);
			if (!result)
				Fail(RequestTimeoutException.FromCorrelationId(_message.CorrelationId));

			Close();

			if (_exception != null)
				throw _exception;

			return result;
		}

		public void SetUnsubscribeAction(UnsubscribeAction unsubscribeAction)

		{
			_completionCallbacks.Add(x => unsubscribeAction());
		}

		public void SetTimeout(TimeSpan timeout)
		{
			_timeout = timeout;
		}

		public void SetTimeoutCallback(Action timeoutCallback)
		{
			_timeoutCallback = timeoutCallback ?? DefaultTimeoutCallback;
		}

		public void Complete<TResponse>(TResponse response)
			where TResponse : class, CorrelatedBy<TKey>
		{
			NotifyComplete();
		}

		public void Fail(Exception exception)
		{
			_exception = exception;

			NotifyComplete();
		}

		public IAsyncResult BeginAsync(AsyncCallback callback, object state)
		{
			if (_waitHandle != null)
			{
				throw new InvalidOperationException("The asynchronous request was already started.");
			}

			_state = state;
			lock (_completionCallbacks)
				_completionCallbacks.Add(callback);

			WaitOrTimerCallback timerCallback = (s, timeoutExpired) =>
				{
					if (timeoutExpired)
					{
						lock (_completionCallbacks)
							_completionCallbacks.Add(asyncResult => _timeoutCallback());

						Fail(RequestTimeoutException.FromCorrelationId(_message.CorrelationId));
					}
				};

			_waitHandle = ThreadPool.RegisterWaitForSingleObject(CompleteEvent, timerCallback, state, _timeout, true);

			return this;
		}

		public void Close()
		{
			if (_waitHandle != null)
			{
				_waitHandle.Unregister(_complete);
				_waitHandle = null;
			}

			lock (_lock)
			{
				if (_complete != null)
				{
					using (_complete)
						_complete.Close();

					_complete = null;
				}
			}
		}

		void NotifyComplete()
		{
			lock (_lock)
			{
				_completed = true;

				if (_complete != null)
					_complete.Set();
			}

			lock (_completionCallbacks)
			{
				_completionCallbacks.Each(callback =>
					{
						try
						{
							callback(this);
						}
						catch (Exception ex)
						{
							_log.Error("The request callback threw an exception", ex);
						}
					});
			}
		}

		static void DefaultTimeoutCallback()
		{
		}
	}
}