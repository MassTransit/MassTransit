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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Transactions;
    using Context;
    using GreenPipes;


    public class TransactionFilter<T> :
        IFilter<T>
        where T : class, PipeContext
    {
        readonly TransactionOptions _options;

        public TransactionFilter(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            TimeSpan timeout = default(TimeSpan))
        {
            if (timeout == default(TimeSpan))
                timeout = TimeSpan.FromSeconds(30);

            _options = new TransactionOptions
            {
                IsolationLevel = isolationLevel,
                Timeout = timeout
            };
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var step = context.CreateFilterScope("transaction");
            step.Add("isolationLevel", _options.IsolationLevel.ToString());
            step.Add("timeout", _options.Timeout);
        }

        [DebuggerNonUserCode]
        public async Task Send(T context, IPipe<T> next)
        {
            SystemTransactionContext systemTransactionContext = null;
            context.GetOrAddPayload<TransactionContext>(() =>
            {
                systemTransactionContext = new SystemTransactionContext(_options);

                return systemTransactionContext;
            });

            try
            {
                await next.Send(context).ConfigureAwait(false);

                if (systemTransactionContext != null)
                    await systemTransactionContext.Commit().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                systemTransactionContext?.Rollback(ex);

                throw;
            }
            finally
            {
                systemTransactionContext?.Dispose();
            }
        }
    }
}