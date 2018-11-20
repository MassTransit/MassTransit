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
namespace MassTransit
{
    using System;
    using System.Transactions;
    using GreenPipes;


    public static class TransactionContextExtensions
    {
        /// <summary>
        /// Create a transaction scope using the transaction context (added by the TransactionFilter),
        /// to ensure that any transactions are carried between any threads.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(this PipeContext context)
        {
            var transactionContext = context.GetPayload<TransactionContext>();

            return new TransactionScope(transactionContext.Transaction);
        }

        /// <summary>
        /// Create a transaction scope using the transaction context (added by the TransactionFilter),
        /// to ensure that any transactions are carried between any threads.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scopeTimeout">The timespan after which the scope times out and aborts the transaction</param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(this PipeContext context, TimeSpan scopeTimeout)
        {
            var transactionContext = context.GetPayload<TransactionContext>();

            return new TransactionScope(transactionContext.Transaction, scopeTimeout);
        }

        /// <summary>
        /// Create a transaction scope using the transaction context (added by the TransactionFilter),
        /// to ensure that any transactions are carried between any threads.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scopeTimeout">The timespan after which the scope times out and aborts the transaction</param>
        /// <param name="asyncFlowOptions">Specifies whether transaction flow across thread continuations is enabled.</param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(this PipeContext context, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlowOptions)
        {
            var transactionContext = context.GetPayload<TransactionContext>();

            return new TransactionScope(transactionContext.Transaction, scopeTimeout, asyncFlowOptions);
        }
    }
}