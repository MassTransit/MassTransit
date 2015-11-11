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
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using Util;


    public class SetPrefetchCountManagementConsumer :
        IConsumer<SetPrefetchCount>
    {
        static readonly ILog _log = Logger.Get<SetPrefetchCountManagementConsumer>();
        readonly IMediator<ISetPrefetchCount> _filterMediator;

        readonly string _queueName;
        DateTime _lastUpdated;

        public SetPrefetchCountManagementConsumer(IMediator<ISetPrefetchCount> filterMediator, string queueName)
        {
            _filterMediator = filterMediator;
            _queueName = queueName;
            _lastUpdated = DateTime.UtcNow;
        }

        public async Task Consume(ConsumeContext<SetPrefetchCount> context)
        {
            if (_queueName.Equals(context.Message.QueueName, StringComparison.OrdinalIgnoreCase))
            {
                if (context.Message.Timestamp >= _lastUpdated)
                {
                    try
                    {
                        await _filterMediator.ForEachAsync(x => x.SetPrefetchCount(context.Message.PrefetchCount));

                        _lastUpdated = context.Message.Timestamp;

                        await context.RespondAsync<PrefetchCountUpdated>(new
                        {
                            Timestamp = DateTime.UtcNow,
                            QueueName = _queueName,
                            context.Message.PrefetchCount
                        });

                        if (_log.IsDebugEnabled)
                            _log.Debug($"Set Prefetch Count: {context.Message.PrefetchCount} ({_queueName})");
                    }
                    catch (Exception exception)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error($"Set Prefetch Count Failed: {context.Message.PrefetchCount} ({_queueName})", exception);

                        throw;
                    }
                }
                else
                    throw new CommandException($"The prefetch count was updated after the command was sent.");
            }
        }
    }
}