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
    using Logging;
    using Messages;

    public class MessageWorkerAvailability<TMessage> :
        IWorkerAvailability<TMessage>,
        Consumes<WorkerAvailable<TMessage>>.Context
        where TMessage : class
    {
        static readonly ILog _log = Logger.Get<MessageWorkerAvailability<TMessage>>();

        readonly IWorkerCache _workerCache;

        public MessageWorkerAvailability(IWorkerCache workerCache)
        {
            _workerCache = workerCache;
        }

        public void Consume(IConsumeContext<WorkerAvailable<TMessage>> context)
        {
            IWorkerInfo<TMessage> worker = _workerCache.GetWorker<TMessage>(context.Message.ControlUri, x =>
                {
                    return new WorkerInfo(context.Message.ControlUri,
                        context.Message.DataUri);
                });

            worker.Update(context.Message.InProgress,
                context.Message.InProgressLimit,
                context.Message.Pending,
                context.Message.PendingLimit,
                context.Message.Updated);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Worker {0}: {1} in progress, {2} pending", worker.DataUri, worker.InProgress,
                    worker.Pending);
        }

        IEnumerable<Action<IConsumeContext<TMessage>>> IWorkerAvailability<TMessage>.GetWorker(
            IConsumeContext<TMessage> context,
            Func<IWorkerInfo<TMessage>, IEnumerable<Action<IConsumeContext<TMessage>>>> selector,
            IWorkerSelector<TMessage> workerSelector)
        {
            return _workerCache.GetAvailableWorkers(context, workerSelector)
                .Take(1)
                .SelectMany(worker =>
                    {
                        worker.Assigned();

                        return selector(worker);
                    });
        }
    }
}