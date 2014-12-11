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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareReceiveQueueFilter :
        IFilter<ModelContext>
    {
        readonly ILog _log = Logger.Get<PrepareReceiveQueueFilter>();
        readonly ReceiveSettings _settings;
        bool _queuePurged;

        public PrepareReceiveQueueFilter(ReceiveSettings settings)
        {
            _settings = settings;
        }

        Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            context.Model.BasicQos(0, _settings.PrefetchCount, false);

            QueueDeclareOk queueOk = context.Model.QueueDeclare(_settings.QueueName, _settings.Durable, _settings.Exclusive,
                _settings.AutoDelete, _settings.QueueArguments);

            string queueName = queueOk.QueueName;

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Queue: {0} ({1})", queueName,
                    string.Join(", ", new[]
                    {
                        _settings.Durable ? "durable" : "",
                        _settings.Exclusive ? "exclusive" : "",
                        _settings.AutoDelete ? "auto-delete" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            if (_settings.PurgeOnStartup)
            {
                if (!_queuePurged)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Purging {0} messages from queue {1}", queueOk.MessageCount, queueName);

                    var purgedMessageCount = context.Model.QueuePurge(queueName);

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Purged {0} messages from queue {1}", purgedMessageCount, queueName);

                    _queuePurged = true;
                }
                else
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Queue {0} already purged at startup, skipping", queueName);
                }
            }

            string exchangeName = _settings.ExchangeName ?? queueName;

            if (!string.IsNullOrWhiteSpace(_settings.ExchangeName) || string.IsNullOrWhiteSpace(_settings.QueueName))
            {
                context.Model.ExchangeDeclare(exchangeName, _settings.ExchangeType, _settings.Durable, _settings.AutoDelete,
                    _settings.ExchangeArguments);

                context.Model.QueueBind(queueName, exchangeName, "");
            }

            ReceiveSettings settings = new RabbitMqReceiveSettings(_settings)
            {
                QueueName = queueName,
                ExchangeName = exchangeName
            };

            context.GetOrAddPayload(() => settings);

            return next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }
    }
}