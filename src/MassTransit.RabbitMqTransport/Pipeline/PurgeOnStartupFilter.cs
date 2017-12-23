// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using RabbitMQ.Client;


    /// <summary>
    /// Purges the queue on startup, only once per filter instance
    /// </summary>
    public class PurgeOnStartupFilter :
        IFilter<ModelContext>
    {
        static readonly ILog _log = Logger.Get<PurgeOnStartupFilter>();
        readonly string _queueName;
        bool _queueAlreadyPurged;

        public PurgeOnStartupFilter(string queueName)
        {
            _queueName = queueName;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("purgeOnStartup");
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            var queueOk = await context.QueueDeclarePassive(_queueName).ConfigureAwait(false);

            await PurgeIfRequested(context, queueOk, _queueName).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task PurgeIfRequested(ModelContext context, QueueDeclareOk queueOk, string queueName)
        {
            if (!_queueAlreadyPurged)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Purging {0} messages from queue {1}", queueOk.MessageCount, queueName);

                var purgedMessageCount = await context.QueuePurge(queueName).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Purged {0} messages from queue {1}", purgedMessageCount, queueName);

                _queueAlreadyPurged = true;
            }
            else
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Queue {0} already purged at startup, skipping", queueName);
            }
        }
    }
}