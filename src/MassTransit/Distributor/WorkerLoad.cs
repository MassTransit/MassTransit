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
    using System.Linq;
    using System.Threading;
    using Logging;
    using Magnum.Extensions;
    using MassTransit.Pipeline;
    using Messages;
    using Stact;
    using Stact.Executors;

    public class MessageWorkerLoad<TMessage> :
        IWorkerLoad<TMessage>,
        Consumes<PingWorker>.Context
        where TMessage : class
    {
        static readonly ILog _log = Logger.Get<MessageWorkerLoad<TMessage>>();
        readonly IPendingMessageTracker<Guid> _pending;
        readonly IWorker _worker;
        int _inProgress;
        int _inProgressLimit = 4;
        int _pendingLimit = 16;
        bool _updatePending;
        Fiber _fiber;

        public MessageWorkerLoad(IWorker worker)
        {
            _fiber = new PoolFiber(new TryCatchOperationExecutor());
            _worker = worker;

            _pending = new WorkerPendingMessageTracker<Guid>();

            PublishWorkerAvailability(1.Minutes());
        }

        public void Consume(IConsumeContext<PingWorker> context)
        {
            try
            {
                var message = new WorkerAvailable<TMessage>(_worker.ControlUri, _worker.DataUri,
                    _inProgress, _inProgressLimit, _pending.PendingMessageCount(), _pendingLimit);

                context.Respond(message);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Worker {0}: {1} in progress, {2} pending", _worker.DataUri,
                        _inProgress, _pending);
            }
            catch
            {
            }
        }


        public IEnumerable<Action<IConsumeContext<Distributed<TMessage>>>> GetWorker(
            IConsumeContext<Distributed<TMessage>> context,
            MultipleHandlerSelector<Distributed<TMessage>> selector)
        {
            _pending.Viewed(context.Message.CorrelationId);

            return selector(context)
                .Select(handler => (Action<IConsumeContext<Distributed<TMessage>>>)(x => Handle(x, handler)));
        }

        public void PublishWorkerAvailability(TimeSpan timeToLive)
        {
            try
            {
                var message = new WorkerAvailable<TMessage>(_worker.ControlUri, _worker.DataUri,
                    _inProgress, _inProgressLimit, _pending.PendingMessageCount(), _pendingLimit);

                _updatePending = false;

                _worker.Bus.Publish(message, x => x.ExpiresAt(DateTime.UtcNow + timeToLive));

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Worker {0}: {1} in progress, {2} pending", _worker.DataUri,
                        _inProgress, _pending);
            }
            catch
            {
            }
        }
        
        void ScheduleUpdate()
        {
            if (!_updatePending)
            {
                _updatePending = true;
                    _fiber.Add(() => PublishWorkerAvailability(10.Seconds()));
            }
        }
        
        void Handle(IConsumeContext<Distributed<TMessage>> context,
            Action<IConsumeContext<Distributed<TMessage>>> handler)
        {
            Interlocked.Increment(ref _inProgress);
            try
            {
                _pending.Consumed(context.Message.CorrelationId);

                handler(context);
            }
            finally
            {
                Interlocked.Decrement(ref _inProgress);
                
                ScheduleUpdate();
            }
            //                RewriteResponseAddress(message.ResponseAddress);
            //            	consumeContext.BaseContext.NotifyConsume(consumeContext, typeof (Worker<TMessage>).ToShortTypeName(),
            //            		message.CorrelationId.ToString());
            //                ScheduleUpdate();
            //                ScheduleWakeUp();
        }
    }
}