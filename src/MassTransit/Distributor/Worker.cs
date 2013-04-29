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
namespace MassTransit.Distributor
{
    using System;
    using System.Threading;
    using Context;
    using Magnum;
    using Magnum.Extensions;
    using Messages;
    using Stact;
    using Stact.Internal;

//    public class Worker<TMessage> :
//        IWorker<TMessage>,
//        Consumes<WakeUpWorker>.All
//        where TMessage : class
//    {
//        readonly IPendingMessageTracker<Guid> _pendingMessages = new WorkerPendingMessageTracker<Guid>();
//        IServiceBus _bus;
//        IServiceBus _controlBus;
//        Uri _controlUri;
//        Uri _dataUri;
//        Func<TMessage, Action<TMessage>> _getConsumer;
//        int _inProgress;
//        int _inProgressLimit = 4;
//        int _pendingLimit = 16;
//        readonly Fiber _fiber = new PoolFiber();
//        UnsubscribeAction _unsubscribeAction = () => false;
//        bool _updatePending;
//        bool _wakeUpPending;
//        Scheduler _scheduler;
//        ScheduledOperation _scheduled;
//
//        public Worker(Func<TMessage, Action<TMessage>> getConsumer)
//            : this(getConsumer, new WorkerSettings())
//        {
//        }
//
//        public Worker(Func<TMessage, Action<TMessage>> getConsumer, WorkerSettings settings)
//        {
//            if(getConsumer == null)
//                throw new ArgumentNullException("getConsumer");
//            if(settings == null)
//                throw new ArgumentNullException("settings");
//
//            _getConsumer = getConsumer;
//
//            _inProgress = 0;
//            _inProgressLimit = settings.InProgressLimit;
//            _pendingLimit = settings.PendingLimit;
//        }
//
//        public void Consume(Distributed<TMessage> message)
//        {
    //            _pendingMessages.Consumed(message.CorrelationId);
    //
    //            Action<TMessage> consumer = _getConsumer(message.Payload);
    //
    //            Interlocked.Increment(ref _inProgress);
    //            try
    //            {
    //                RewriteResponseAddress(message.ResponseAddress);
    //
    //                consumer(message.Payload);
    //
    //            	var consumeContext = _bus.MessageContext<Distributed<TMessage>>();
    //
    //            	consumeContext.BaseContext.NotifyConsume(consumeContext, typeof (Worker<TMessage>).ToShortTypeName(),
    //            		message.CorrelationId.ToString());
    //            }
    //            finally
    //            {
    //                Interlocked.Decrement(ref _inProgress);
    //
    //                ScheduleUpdate();
    //                ScheduleWakeUp();
    //
    //                var disposal = consumer as IDisposable;
    //                if (disposal != null)
    //                {
    //                    disposal.Dispose();
    //                }
    //            }
//        }
//
//        public bool Accept(Distributed<TMessage> message)
//        {
//            if (_inProgress >= _inProgressLimit)
//            {
//                _pendingMessages.Viewed(message.CorrelationId);
//                return false;
//            }
//
//            return true;
//        }
//
//        void Consume(IConsumeContext<PingWorker> context)
//        {
//            try
//            {
//                var message = new WorkerAvailable<TMessage>(_controlUri, _dataUri, _inProgress, _inProgressLimit,
//                    _pendingMessages.PendingMessageCount(), _pendingLimit);
//                _updatePending = false;
//
//                context.Respond(message);
//            }
//            catch
//            {
//            }
//        }
//
//        public void Consume(WakeUpWorker message)
//        {
//            _wakeUpPending = false;
//        }
//
//        bool _disposed;
//        TimeSpan _availabilityInterval;
//
//        public void Dispose()
//        {
//            Dispose(true);
//        }
//
//        void Dispose(bool disposing)
//        {
//            if (_disposed) return;
//            if (disposing)
//            {
//                Stop();
//                _fiber.Stop();
//
//                _controlBus = null;
//                _getConsumer = null;
//            }
//
//            _disposed = true;
//        }
//
//        public void Start(IServiceBus bus)
//        {
//            _bus = bus;
//            _controlBus = bus.ControlBus;
//
//            _dataUri = _bus.Endpoint.Address.Uri;
//            _controlUri = _controlBus.Endpoint.Address.Uri;
//
//            _unsubscribeAction = bus.ControlBus.SubscribeHandler<ConfigureWorker>(Consume, Accept);
//            _unsubscribeAction += bus.ControlBus.SubscribeContextHandler<PingWorker>(x => Consume(x));
//
//            _unsubscribeAction += bus.SubscribeInstance(this);
//
//            _scheduler = new TimerScheduler(new PoolFiber());
//
//            _availabilityInterval = 3.Seconds();
//            _scheduled = _scheduler.Schedule(_availabilityInterval, _availabilityInterval, _fiber, PublishWorkerAvailability);
//        }
//
//        public void Stop()
//        {
//            if (_scheduled != null)
//            {
//                _scheduled.Cancel();
//                _scheduled = null;
//            }
//
//            if (_scheduler != null)
//            {
//                _scheduler.Stop(60.Seconds());
//                _scheduler = null;
//            }
//
//            if (_fiber != null)
//            {
//                _fiber.Shutdown(60.Seconds());
//            }
//
//            if (_unsubscribeAction != null)
//            {
//                _unsubscribeAction();
//                _unsubscribeAction = null;
//            }
//        }
//
//        bool Accept(ConfigureWorker message)
//        {
//            return GetType().GetGenericArguments()[0].FullName == message.MessageType;
//        }
//
//        void Consume(ConfigureWorker message)
//        {
//            if (message.InProgressLimit >= 0)
//                _inProgressLimit = message.InProgressLimit;
//
//            if (message.PendingLimit >= 0)
//                _pendingLimit = message.PendingLimit;
//
//            ScheduleUpdate();
//        }
//
//        void ScheduleWakeUp()
//        {
//            if (!_wakeUpPending)
//            {
//                _wakeUpPending = true;
//                _fiber.Add(() =>
//                    {
//                        try
//                        {
//                            _bus.Endpoint.Send(new WakeUpWorker());
//                        }
//                        catch
//                        {
//                        }
//                    });
//            }
//        }
//
//        void ScheduleUpdate()
//        {
//            if (!_updatePending)
//            {
//                _updatePending = true;
//                try
//                {
//                    _fiber.Add(PublishWorkerAvailability);
//                }
//                catch
//                {
//                }
//            }
//        }
//
//        void PublishWorkerAvailability()
//        {
//            try
//            {
//                var message = new WorkerAvailable<TMessage>(_controlUri, _dataUri, _inProgress, _inProgressLimit,
//                    _pendingMessages.PendingMessageCount(), _pendingLimit);
//                _updatePending = false;
//
//                _bus.Publish(message, context =>
//                    {
//                        context.SetExpirationTime(SystemUtil.UtcNow + _availabilityInterval);
//                    });
//            }
//            catch
//            {
//            }
//        }
//
//        static void RewriteResponseAddress(Uri responseAddress)
//        {
//            var context = ContextStorage.MessageContext<Distributed<TMessage>>() as ConsumeContext<Distributed<TMessage>>;
//            if (context != null)
//            {
//                context.SetResponseAddress(responseAddress);
//            }
//        }
//    }
}