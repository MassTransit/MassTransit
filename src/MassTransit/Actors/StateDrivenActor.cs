// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Actors
{
	using System;
	using System.Threading;
	using Magnum;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Saga;
	using Util;

	public class StateDrivenActor<T> :
		SagaStateMachine<T>,
		ISaga,
		IAsyncResult
		where T : SagaStateMachine<T>
	{
		private readonly AsyncCallback _callback;
		private readonly CommandQueue _queue = new ThreadPoolCommandQueue();
		private readonly object _state;
		private volatile bool _completed;

		protected StateDrivenActor()
			: this(CombGuid.Generate(), null, null)
		{
		}

		protected StateDrivenActor(Guid correlationId, AsyncCallback callback, object state)
		{
			CorrelationId = correlationId;
			_callback = callback;
			_state = state;
		}

		protected StateDrivenActor(AsyncCallback callback, object state)
			: this(CombGuid.Generate(), callback, state)
		{
			_callback = callback;
			_state = state;
		}

		protected StateDrivenActor(Guid correlationId)
			: this(correlationId, null, null)
		{
		}

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { throw new NotImplementedException("Wait Handles Are Not Supported With Actors"); }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		[Indexed]
		public virtual Guid CorrelationId { get; set; }

		public virtual IServiceBus Bus { get; set; }

		protected void SetAsCompleted()
		{
			_queue.Enqueue(() =>
				{
					_completed = true;

					if (_callback != null)
						_callback(this);
				});
		}
	}
}