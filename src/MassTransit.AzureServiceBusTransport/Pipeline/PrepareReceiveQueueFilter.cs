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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareReceiveQueueFilter :
        IFilter<ConnectionContext>
    {
        readonly ILog _log = Logger.Get<PrepareReceiveQueueFilter>();
        readonly ReceiveSettings _settings;
        bool _queuePurged;

        public PrepareReceiveQueueFilter(ReceiveSettings settings)
        {
            _settings = settings;
        }

        public async Task Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            QueueDescription queueDescription = null;
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Creating queue {0}", _settings.QueueDescription.Path);

                queueDescription = context.NamespaceManager.CreateQueue(_settings.QueueDescription);
            }
            catch (MessagingEntityAlreadyExistsException)
            {
            }
            if (queueDescription == null)
                queueDescription = context.NamespaceManager.GetQueue(_settings.QueueDescription.Path);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Queue: {0} ({1})", queueDescription.Path,
                    string.Join(", ", new[]
                    {
                        queueDescription.EnableExpress ? "express" : "",
                        queueDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        queueDescription.EnableDeadLetteringOnMessageExpiration ? "dead letter" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            if (_settings.PurgeOnStartup && !_queuePurged)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Purging {0} messages from queue {1}", queueDescription.MessageCount, queueDescription.Path);

                await PurgeQueue(context, queueDescription.Path);

                _queuePurged = true;
            }

            context.GetOrAddPayload(() => _settings);

            await next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }

        async Task PurgeQueue(ConnectionContext context, string queueName)
        {
            QueueClient client = context.Factory.CreateQueueClient(queueName, ReceiveMode.ReceiveAndDelete);
            client.PrefetchCount = 1000;

            BrokeredMessage message = await client.ReceiveAsync();
            while (message != null)
            {
                await message.CompleteAsync();

                message = await client.ReceiveAsync();
            }
        }
    }
}