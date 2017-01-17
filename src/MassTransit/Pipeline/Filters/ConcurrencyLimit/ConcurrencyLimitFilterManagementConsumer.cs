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
namespace MassTransit.Pipeline.Filters.ConcurrencyLimit
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using Logging;


    /// <summary>
    /// Consumer which when connected to a management endpoint can control the concurrency
    /// limit.
    /// </summary>
    public class ConcurrencyLimitFilterManagementConsumer :
        IConsumer<SetConcurrencyLimit>
    {
        static readonly ILog _log = Logger.Get<ConcurrencyLimitFilterManagementConsumer>();
        readonly IPipeRouter _router;
        readonly string _id;
        DateTime _lastUpdated;

        public ConcurrencyLimitFilterManagementConsumer(IPipeRouter router, string id = null)
        {
            _router = router;
            _id = id;

            _lastUpdated = DateTime.UtcNow;
        }

        public async Task Consume(ConsumeContext<SetConcurrencyLimit> context)
        {
            if (_id == null || _id.Equals(context.Message.Id, StringComparison.OrdinalIgnoreCase))
            {
                if (context.Message.Timestamp >= _lastUpdated)
                {
                    try
                    {
                        await _router.SetConcurrencyLimit(context.Message.ConcurrencyLimit).ConfigureAwait(false);

                        _lastUpdated = context.Message.Timestamp;

                        await context.RespondAsync<ConcurrencyLimitUpdated>(new
                        {
                            Timestamp = DateTime.UtcNow,
                            context.Message.Id,
                            context.Message.ConcurrencyLimit
                        }).ConfigureAwait(false);

                        if (_log.IsDebugEnabled)
                            _log.Debug($"Set Consumer Limit: {context.Message.ConcurrencyLimit} ({context.Message.Id ?? ""})");
                    }
                    catch (Exception exception)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error($"Set Consumer Limit Failed: {context.Message.ConcurrencyLimit} ({context.Message.Id})", exception);

                        throw;
                    }
                }
                else
                    throw new CommandException("The concurrency limit was updated after the command was sent.");
            }
        }
    }
}