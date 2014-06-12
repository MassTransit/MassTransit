// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using Messages;


    public class MessageWorkerAvailability<TMessage> :
        IWorkerAvailability<TMessage>,
        IConsumer<WorkerAvailable<TMessage>>
        where TMessage : class
    {
        static readonly ILog _log = Logger.Get<MessageWorkerAvailability<TMessage>>();

        readonly IWorkerCache _workerCache;

        public MessageWorkerAvailability(IWorkerCache workerCache)
        {
            _workerCache = workerCache;
        }

        public async Task Consume(ConsumeContext<WorkerAvailable<TMessage>> context)
        {
            IWorkerInfo<TMessage> worker = _workerCache.GetWorker<TMessage>(context.Message.ControlUri, x =>
            {
                if (_log.IsInfoEnabled)
                    _log.InfoFormat("Discovered New Worker: {0}", context.Message.ControlUri);

                var workerInfo = new WorkerInfo(context.Message.ControlUri, context.Message.DataUri);

                return new WorkerInfo<TMessage>(workerInfo);
            });

            worker.Update(context.Message.InProgress,
                context.Message.InProgressLimit,
                context.Message.Pending,
                context.Message.PendingLimit,
                context.Message.Updated);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Worker {0}: {1} in progress, {2} pending", worker.DataUri, worker.InProgress,
                    worker.Pending);
            }
        }

        IWorkerInfo<TMessage> IWorkerAvailability<TMessage>.GetWorker(ConsumeContext<TMessage> context,
            IWorkerSelector<TMessage> workerSelector)
        {
            return _workerCache.GetAvailableWorkers(context, workerSelector).FirstOrDefault();
        }
    }
}