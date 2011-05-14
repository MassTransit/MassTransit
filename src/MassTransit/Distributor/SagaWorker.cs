// Copyright 2007-2011 The Apache Software Foundation.
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
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Messages;
	using Saga;
	using Stact;
	using Stact.Internal;

	public class SagaWorker<TSaga> :
		ISagaWorker<TSaga>,
		Consumes<WakeUpWorker>.All
		where TSaga : SagaStateMachine<TSaga>, ISaga
	{
		readonly ISagaRepository<TSaga> _sagaRepository;
		private readonly IList<Type> _messageTypes = new List<Type>();
		private readonly IPendingMessageTracker<Guid> _pendingMessages = new WorkerPendingMessageTracker<Guid>();
		private readonly Fiber _fiber = new PoolFiber();
		private IServiceBus _bus;
		private IServiceBus _controlBus;
		private Uri _controlUri;
		private Uri _dataUri;
		private int _inProgress;
		private int _inProgressLimit = 4;
		private int _pendingLimit = 16;
		private UnsubscribeAction _unsubscribeAction = () => false;
		private bool _updatePending;
		private Scheduler _scheduler;
		private ScheduledOperation _scheduled;

		public SagaWorker(ISagaRepository<TSaga> sagaRepository)
			: this(sagaRepository, new WorkerSettings())
		{
		}

		public SagaWorker(ISagaRepository<TSaga> sagaRepository, WorkerSettings settings)
		{
			_sagaRepository = sagaRepository;
			_inProgress = 0;
			_inProgressLimit = settings.InProgressLimit;
			_pendingLimit = settings.PendingLimit;
		}

		public void Consume(WakeUpWorker message)
		{
		}

		public void Dispose()
		{
			Stop();

			_fiber.Stop();

			_controlBus = null;
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

            //REVIEW: should this be IEndpoint?
			_dataUri = _bus.Endpoint.Address.Uri;
			_controlUri = _controlBus.Endpoint.Address.Uri;

			_unsubscribeAction = bus.ControlBus.SubscribeHandler<ConfigureWorker>(Consume, Accept);
			_unsubscribeAction += bus.SubscribeInstance(this);
			_unsubscribeAction += bus.SubscribeSagaWorker(this, _sagaRepository);

			CacheMessageTypesForSaga();

			_scheduler = new TimerScheduler(new PoolFiber());
			_scheduled = _scheduler.Schedule(3.Seconds(), 1.Minutes(), _fiber, PublishWorkerAvailability);
		}

		public void Stop()
		{
			if (_scheduled != null)
			{
				_scheduled.Cancel();
				_scheduled = null;
			}

			if (_scheduler != null)
			{
				_scheduler.Stop(60.Seconds());
				_scheduler = null;
			}

			if (_fiber != null)
			{
				_fiber.Shutdown(60.Seconds());
			}

			if(_unsubscribeAction != null)
			{
				_unsubscribeAction();
				_unsubscribeAction = null;
			}
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
			return typeof (TSaga).GetType().FullName == message.MessageType;
		}

		private void Consume(ConfigureWorker message)
		{
			if (message.InProgressLimit >= 0)
				_inProgressLimit = message.InProgressLimit;

			if (message.PendingLimit >= 0)
				_pendingLimit = message.PendingLimit;

			ScheduleUpdate();
		}

		private void ScheduleUpdate()
		{
			if (!_updatePending)
			{
				_updatePending = true;
				_fiber.Add(PublishWorkerAvailability);
			}
		}

		private void PublishWorkerAvailability()
		{
			try
			{
				_updatePending = false;

				_messageTypes.Each(type => { this.FastInvoke(new[] {type}, "PublishWorkerAvailable"); });
			}
			catch
			{
			}
		}

		private void PublishWorkerAvailable<TMessage>()
		{
			_bus.Publish(new WorkerAvailable<TMessage>(_controlUri, _dataUri, _inProgress, _inProgressLimit, _pendingMessages.PendingMessageCount(), _pendingLimit));
		}
	}
}