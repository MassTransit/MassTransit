// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Management
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using MassTransit.Pipeline.Pipes;


    public class SetPrefetchCountManagementConsumer :
        IConsumer<SetPrefetchCount>
    {
        static readonly ILog _log = Logger.Get<SetPrefetchCountManagementConsumer>();

        readonly IManagementPipe _managementPipe;
        readonly string _queueName;
        DateTime _lastUpdated;

        public SetPrefetchCountManagementConsumer(IManagementPipe managementPipe, string queueName)
        {
            _managementPipe = managementPipe;
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
                        await _managementPipe.Send(context).ConfigureAwait(false);

                        _lastUpdated = context.Message.Timestamp;

                        await context.RespondAsync<PrefetchCountUpdated>(new
                        {
                            Timestamp = DateTime.UtcNow,
                            QueueName = _queueName,
                            context.Message.PrefetchCount
                        }).ConfigureAwait(false);

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