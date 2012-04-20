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
    using Logging;
    using MassTransit.Pipeline;
    using Messages;

    public class MessageWorkerLoad<TMessage> :
        IWorkerLoad<TMessage>,
        Consumes<PingWorker>.Context
        where TMessage : class
    {
        static readonly ILog _log = Logger.Get<MessageWorkerLoad<TMessage>>();
        static IPendingMessageTracker<TMessage> _pending;
        readonly IWorker _worker;
        int _inProgress;
        int _inProgressLimit = 4;
        int _pendingLimit = 16;

        public MessageWorkerLoad(IWorker worker)
        {
            _worker = worker;

            _pending = new WorkerPendingMessageTracker<TMessage>();
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


        //                    _pendingMessages.Consumed(message.CorrelationId);
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
        public IEnumerable<Action<IConsumeContext<Distributed<TMessage>>>> GetWorker(
            IConsumeContext<Distributed<TMessage>> context,
            MultipleHandlerSelector<Distributed<TMessage>> selector)
        {
            foreach (var handler in selector(context))
                yield return ctx => Handle(ctx, handler);
        }

        void Handle(IConsumeContext<Distributed<TMessage>> context,
            Action<IConsumeContext<Distributed<TMessage>>> handler)
        {
            handler(context);
        }
    }
}