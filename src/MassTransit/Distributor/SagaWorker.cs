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
	using System.Collections.Generic;
	using System.Threading;
	using Magnum;
	using Magnum.Reflection;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Messages;
	using Saga;

	public class SagaWorker<TSaga> :
		ISagaWorker<TSaga>,
		Consumes<WakeUpWorker>.All
		where TSaga : SagaStateMachine<TSaga>, ISaga
	{
		private readonly IList<Type> _messageTypes = new List<Type>();
		private readonly IPendingMessageTracker<Guid> _pendingMessages = new WorkerPendingMessageTracker<Guid>();
		private readonly CommandQueue _queue = new ThreadPoolCommandQueue();
		private IServiceBus _bus;
		private IServiceBus _controlBus;
		private Uri _controlUri;
		private Uri _dataUri;
		private int _inProgress;
		private int _inProgressLimit = 4;
		private int _pendingLimit = 16;
		private UnsubscribeAction _unsubscribeAction = () => false;
		private bool _updatePending;

		public SagaWorker()
			: this(new WorkerSettings())
		{
		}

		public SagaWorker(WorkerSettings settings)
		{
			_inProgress = 0;
			_inProgressLimit = settings.InProgressLimit;
			_pendingLimit = settings.PendingLimit;
		}

		public void Consume(WakeUpWorker message)
		{
		}

		public void Dispose()
		{
			_controlBus = null;
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			_dataUri = _bus.Endpoint.Uri;
			_controlUri = _controlBus.Endpoint.Uri;

			_unsubscribeAction = bus.ControlBus.Subscribe<ConfigureWorker>(Consume, Accept);
			_unsubscribeAction += bus.Subscribe(this);

			CacheMessageTypesForSaga();

			PublishWorkerAvailability();
		}

		public void Stop()
		{
			_unsubscribeAction();
		}

		public bool CanAcceptMessage<TMessage>(Distributed<TMessage> message)
		{
			if (_inProgress >= _inProgressLimit)
			{
				_pendingMessages.Viewed(message.CorrelationId);

				return false;
			}

			return true;
		}

		public void ConsumingMessage<TMessage>(Distributed<TMessage> message)
		{
			_pendingMessages.Consumed(message.CorrelationId);
		}

		public void IncrementInProgress()
		{
			Interlocked.Increment(ref _inProgress);
		}

		public void DecrementInProgress()
		{
			Interlocked.Decrement(ref _inProgress);

			if (_inProgress == 0)
				_bus.Endpoint.Send(new WakeUpWorker());

			PublishWorkerAvailability();
		}

		private void CacheMessageTypesForSaga()
		{
			TSaga saga = FastActivator<TSaga>.Create(CombGuid.Generate());

			saga.EnumerateDataEvents(type => _messageTypes.Add(type));
		}

        private bool Accept(ConfigureWorker message)
        {
            return typeof(TSaga).GetType().FullName == message.MessageType;
        }

		private void Consume(ConfigureWorker message)
		{
			if (message.InProgressLimit >= 0)
				_inProgressLimit = message.InProgressLimit;

			if (message.PendingLimit >= 0)
				_pendingLimit = message.PendingLimit;

			PublishWorkerAvailability();
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

			_messageTypes.Each(type => { this.FastInvoke(new[] {type}, "PublishWorkerAvailable"); });
		}

		private void PublishWorkerAvailable<TMessage>()
		{
			_bus.Publish(new WorkerAvailable<TMessage>(_controlUri, _dataUri, _inProgress, _inProgressLimit, _pendingMessages.PendingMessageCount(), _pendingLimit));
		}
	}
}