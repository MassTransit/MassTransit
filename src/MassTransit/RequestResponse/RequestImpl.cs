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
	using System.Linq;
	using System.Threading;
	using Exceptions;
	using Magnum.Extensions;

	public class RequestImpl<TRequest, TKey> :
		IRequest<TRequest, TKey>
		where TRequest : class, CorrelatedBy<TKey>
	{
		readonly ManualResetEvent _complete = new ManualResetEvent(false);
		TimeSpan _timeout = TimeSpan.MaxValue;
		object _state;
		Action _timeoutCallback;
		RegisteredWaitHandle _waitHandle;
		Exception _exception;

		bool _completed;
		readonly IList<AsyncCallback> _completionCallbacks;

		public RequestImpl()
		{
			_completionCallbacks = new List<AsyncCallback>();
		}

		public void SetTimeout(TimeSpan timeout)
		{
			_timeout = timeout;
		}

		public void SetTimeoutCallback(Action timeoutCallback)
		{
			_timeoutCallback = timeoutCallback;
		}

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return _complete; }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public void WaitForIt()
		{
			UnsubscribeAction unsubscribeToken = () => true;
			try
			{
			//	unsubscribeToken = SubscribeToResponseMessages(unsubscribeToken);

				if (WaitForResponseAction() == false)
				{
					if (_timeoutCallback != null)
						_timeoutCallback();
				}
			}
			finally
			{
				unsubscribeToken();
			}
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

		void NotifyComplete()
		{
			_completed = true;
			_complete.Set();

			_completionCallbacks.Each(callback =>
				{
					try
					{
						callback(this);
					}
					catch (Exception)
					{
					}
				});
		}

		public void BeginAsyncRequest(AsyncCallback callback, object state)
		{
			if(_waitHandle != null)
			{
				throw new InvalidOperationException("The asynchronous request was already started.");
			}

			_state = state;
			lock(_completionCallbacks)
				_completionCallbacks.Add(callback);

			WaitOrTimerCallback timerCallback = (s, timeoutExpired) =>
				{
					if (timeoutExpired)
					{
						lock(_completionCallbacks)
							_completionCallbacks.Add(asyncResult => _timeoutCallback());
					}

					Fail(new RequestTimeoutException());
				};

			_waitHandle = ThreadPool.RegisterWaitForSingleObject(_complete, timerCallback, state, _timeout, true);
		}

		bool WaitForResponseAction()
		{
			return _complete.WaitOne(_timeout, true);
		}


		public bool Wait(TimeSpan timeout)
		{
			_timeout = timeout;

			return Wait();
		}

		public bool Wait()
		{
			var result = _complete.WaitOne(_timeout, true);

			Close();

			if(_exception != null)
				throw _exception;

			return result;
		}

		~RequestImpl()
		{
			Close();
		}

		public void Close()
		{
			if (_waitHandle != null)
			{
				_waitHandle.Unregister(_complete);
				_waitHandle = null;
			}

			using (_complete)
				_complete.Close();
		}

		public void AddCompletionCallback(Action callback)
		{
			lock(_completionCallbacks)
				_completionCallbacks.Add(asyncResult => callback());
		}
	}
}