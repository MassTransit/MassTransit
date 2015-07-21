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
namespace MassTransit.Context
{
    using System;
    using System.Transactions;


    public class SystemTransactionContext :
        TransactionContext,
        IDisposable
    {
        readonly CommittableTransaction _transaction;
        bool _completed;
        bool _disposed;

        public SystemTransactionContext(TransactionOptions options)
        {
            _transaction = new CommittableTransaction(options);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _transaction.Dispose();

            _disposed = true;
        }

        Transaction TransactionContext.Transaction => _transaction;

        void TransactionContext.Commit()
        {
            if (_completed)
                return;

            _transaction.Commit();

            _completed = true;
        }

        void TransactionContext.Rollback()
        {
            if (_completed)
                return;

            _transaction.Rollback();

            _completed = true;
        }

        void TransactionContext.Rollback(Exception exception)
        {
            if (_completed)
                return;

            _transaction.Rollback(exception);

            _completed = true;
        }
    }
}