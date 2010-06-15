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
namespace MassTransit.Distributor
{
	using System;
	using System.Threading;
	using Internal;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Magnum.Actors.Schedulers;
	using Magnum.DateTimeExtensions;
	using Messages;

	public class Worker<T> :
		IWorker<T>,
		Consumes<WakeUpWorker>.All,
		Consumes<PingWorker>.All,
		Consumes<Distributed<T>>.Selected
		where T : class
	{
		private readonly IPendingMessageTracker<Guid> _pendingMessages = new WorkerPendingMessageTracker<Guid>();
		private IServiceBus _bus;
		private IServiceBus _controlBus;
		private Uri _controlUri;
		private Uri _dataUri;
		private Func<T, Action<T>> _getConsumer;
		private int _inProgress;
		private int _inProgressLimit = 4;
		private int _pendingLimit = 16;
		private readonly CommandQueue _queue = new ThreadPoolCommandQueue();
		private UnsubscribeAction _unsubscribeAction = () => false;
		private bool _updatePending;
		private bool _wakeUpPending;
	    private ThreadPoolScheduler _threadPoolScheduler;

		public Worker(Func<T, Action<T>> getConsumer)
			: this(getConsumer, new WorkerSettings())
		{
		}

		public Worker(Func<T, Action<T>> getConsumer, WorkerSettings settings)
		{
			_getConsumer = getConsumer;

			_inProgress = 0;
			_inProgressLimit = settings.InProgressLimit;
			_pendingLimit = settings.PendingLimit;
		}

		public void Consume(Distributed<T> message)
		{
			_pendingMessages.Consumed(message.CorrelationId);

			Action<T> consumer = _getConsumer(message.Payload);

			Interlocked.Increment(ref _inProgress);
			try
			{
				RewriteResponseAddress(message.ResponseAddress);

				consumer(message.Payload);
			}
			finally
			{
				Interlocked.Decrement(ref _inProgress);

				ScheduleUpdate();
				ScheduleWakeUp();

				var disposal = consumer as IDisposable;
				if (disposal != null)
				{
					disposal.Dispose();
				}
			}
		}

		public bool Accept(Distributed<T> message)
		{
			if (_inProgress >= _inProgressLimit)
			{
				_pendingMessages.Viewed(message.CorrelationId);
				return false;
			}

			return true;
		}

		public void Consume(PingWorker message)
		{
			PublishWorkerAvailability();
		}

		public void Consume(WakeUpWorker message)
		{
			_wakeUpPending = false;
		}

		public void Dispose()
		{
			_controlBus = null;
			_getConsumer = null;
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			_dataUri = _bus.Endpoint.Uri;
			_controlUri = _controlBus.Endpoint.Uri;

			_unsubscribeAction = bus.ControlBus.Subscribe<ConfigureWorker>(Consume, Accept);
			_unsubscribeAction += bus.ControlBus.Subscribe<PingWorker>(Consume);
			_unsubscribeAction += bus.Subscribe(this);

            _threadPoolScheduler = new ThreadPoolScheduler();

		    _threadPoolScheduler.Schedule((int) 3.Seconds().TotalMilliseconds, (int) 1.Minutes().TotalMilliseconds, PublishWorkerAvailability);
		}

	    public void Stop()
		{
	        _threadPoolScheduler.Dispose();
			_unsubscribeAction();
		}

        private bool Accept(ConfigureWorker message)
        {
            return GetType().GetGenericArguments()[0].FullName == message.MessageType;
        }

		private void Consume(ConfigureWorker message)
		{
			if (message.InProgressLimit >= 0)
				_inProgressLimit = message.InProgressLimit;

			if (message.PendingLimit >= 0)
				_pendingLimit = message.PendingLimit;

			PublishWorkerAvailability();
		}

		private void ScheduleWakeUp()
		{
			if (!_wakeUpPending)
			{
				_wakeUpPending = true;
				_queue.Enqueue(() => _bus.Endpoint.Send(new WakeUpWorker()));
			}
		}

		private void ScheduleUpdate()
		{
			if (!_updatePending)
			{
				_updatePending = true;
				_queue.Enqueue(PublishWorkerAvailability);
			}
		}

		private void PublishWorkerAvailability()
		{
			_updatePending = false;

			_bus.Publish(new WorkerAvailable<T>(_controlUri, _dataUri, _inProgress, _inProgressLimit, _pendingMessages.PendingMessageCount(), _pendingLimit));
		}

		private static void RewriteResponseAddress(Uri responseAddress)
		{
			InboundMessageHeaders.SetCurrent(x => x.SetResponseAddress(responseAddress));
		}
	}
}