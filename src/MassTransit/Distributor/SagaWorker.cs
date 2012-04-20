// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
        readonly Fiber _fiber;
        readonly IList<Type> _messageTypes;
        readonly IPendingMessageTracker<Guid> _pendingMessages;
        readonly ISagaRepository<TSaga> _sagaRepository;
        IServiceBus _bus;
        IServiceBus _controlBus;
        Uri _controlUri;
        Uri _dataUri;
        int _inProgress;
        int _inProgressLimit = 4;
        int _pendingLimit = 16;
        ScheduledOperation _scheduled;
        Scheduler _scheduler;
        UnsubscribeAction _unsubscribeAction = () => false;
        bool _updatePending;

        public SagaWorker(ISagaRepository<TSaga> sagaRepository)
            : this(sagaRepository, new WorkerSettings())
        {
        }

        public SagaWorker(ISagaRepository<TSaga> sagaRepository, WorkerSettings settings)
        {
            _fiber = new PoolFiber();
            _messageTypes = new List<Type>();
            _pendingMessages = new WorkerPendingMessageTracker<Guid>();
            if (sagaRepository == null)
                throw new ArgumentNullException("sagaRepository");
            if (settings == null)
                throw new ArgumentNullException("settings");

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

            _dataUri = _bus.Endpoint.Address.Uri;
            _controlUri = _controlBus.Endpoint.Address.Uri;

            _unsubscribeAction = bus.ControlBus.SubscribeHandler<ConfigureWorker>(Consume, Accept);
            _unsubscribeAction += bus.ControlBus.SubscribeContextHandler<PingWorker>(Consume);
            _unsubscribeAction += bus.SubscribeInstance(this);
           // _unsubscribeAction += bus.SubscribeSagaWorker(this, _sagaRepository);

            CacheMessageTypesForSaga();

            _scheduler = new TimerScheduler(new PoolFiber());
            _scheduled = _scheduler.Schedule(3.Seconds(), 3.Seconds(), _fiber, PublishWorkerAvailability);
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

            if (_unsubscribeAction != null)
            {
                _unsubscribeAction();
                _unsubscribeAction = null;
            }
        }

        void Consume(IConsumeContext<PingWorker> context)
        {
            try
            {
                _messageTypes.Each(type => this.FastInvoke(new[] { type }, "RespondToPingWorker", context));
            }
            catch
            {
            }
        }

        void RespondToPingWorker<TMessage>(IConsumeContext<PingWorker> context)
        {
            var message = new WorkerAvailable<TMessage>(_controlUri, _dataUri, _inProgress, _inProgressLimit,
                _pendingMessages.PendingMessageCount(), _pendingLimit);

            context.Respond(message);
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

        void CacheMessageTypesForSaga()
        {
            TSaga saga = FastActivator<TSaga>.Create(NewId.NextGuid());

            saga.EnumerateDataEvents(type => _messageTypes.Add(type));
        }

        bool Accept(ConfigureWorker message)
        {
            return typeof(TSaga).GetType().FullName == message.MessageType;
        }

        void Consume(ConfigureWorker message)
        {
            if (message.InProgressLimit >= 0)
                _inProgressLimit = message.InProgressLimit;

            if (message.PendingLimit >= 0)
                _pendingLimit = message.PendingLimit;

            ScheduleUpdate();
        }

        void ScheduleUpdate()
        {
            if (!_updatePending)
            {
                _updatePending = true;
                _fiber.Add(PublishWorkerAvailability);
            }
        }

        void PublishWorkerAvailability()
        {
            try
            {
                _updatePending = false;

                _messageTypes.Each(type => this.FastInvoke(new[] {type}, "PublishWorkerAvailable"));
            }
            catch
            {
            }
        }

        void PublishWorkerAvailable<TMessage>()
        {
            _bus.Publish(new WorkerAvailable<TMessage>(_controlUri, _dataUri, _inProgress, _inProgressLimit,
                _pendingMessages.PendingMessageCount(), _pendingLimit));
        }
    }
}