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
    using Context;
    using Contracts;
    using GreenPipes;


    /// <summary>
    /// Consumer which when connected to a management endpoint can control the concurrency
    /// limit.
    /// </summary>
    public class ConcurrencyLimitFilterManagementConsumer :
        IConsumer<SetConcurrencyLimit>
    {
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

                        LogContext.Debug?.Log("Set Consumer Limit: {ConcurrencyLimit} ({CommandId})", context.Message.ConcurrencyLimit, context.Message.Id);
                    }
                    catch (Exception exception)
                    {
                        LogContext.Error?.Log(exception, "Set Consumer Limit failed: {ConcurrencyLimit} ({CommandId})", context.Message.ConcurrencyLimit,
                            context.Message.Id);

                        throw;
                    }
                }
                else
                    throw new CommandException("The concurrency limit was updated after the command was sent.");
            }
        }
    }
}
