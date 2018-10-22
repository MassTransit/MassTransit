// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;


    /// <summary>
    /// Purges the queue on startup, only once per filter instance
    /// </summary>
    public class PurgeOnStartupFilter :
        IFilter<ClientContext>
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

        async Task IFilter<ClientContext>.Send(ClientContext context, IPipe<ClientContext> next)
        {
            await PurgeIfRequested(context, _queueName).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task PurgeIfRequested(ClientContext context, string queueName)
        {
            if (!_queueAlreadyPurged)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Purging queue {1}", queueName);

                await context.PurgeQueue(queueName, context.CancellationToken).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Purged queue {1}", queueName);

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