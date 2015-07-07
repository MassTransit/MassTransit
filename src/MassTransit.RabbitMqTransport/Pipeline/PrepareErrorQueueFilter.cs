// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Monitoring.Introspection;
    using RabbitMQ.Client;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareErrorQueueFilter :
        IFilter<ModelContext>
    {
        readonly ILog _log = Logger.Get<PrepareSendExchangeFilter>();

        readonly ErrorQueueSettings _settings;

        public PrepareErrorQueueFilter(ErrorQueueSettings settings)
        {
            _settings = settings;
        }

        async void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            if (!context.HasPayloadType(typeof(ErrorQueueSettings)))
                await DeclareAndBindQueue(context);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task DeclareAndBindQueue(ModelContext context)
        {
            QueueDeclareOk queueOk = await context.QueueDeclare(_settings.ExchangeName, _settings.Durable, false,
                _settings.AutoDelete, new Dictionary<string, object>());

            string queueName = queueOk.QueueName;

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Queue: {0} ({1})", queueName,
                    string.Join(", ", new[]
                    {
                        _settings.Durable ? "durable" : "",
                        _settings.AutoDelete ? "auto-delete" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            await context.QueueBind(queueName, _settings.ExchangeName, "", new Dictionary<string, object>());

            context.GetOrAddPayload(() => _settings);
        }
    }
}