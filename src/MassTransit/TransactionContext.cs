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


    public interface TransactionContext
    {
        /// <summary>
        /// Returns the current transaction scope, creating a dependent scope if a thread switch
        /// occurred
        /// </summary>
        Transaction Transaction { get; }

        /// <summary>
        /// Complete the transaction scope
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback the transaction
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rollback the transaction
        /// </summary>
        /// <param name="exception">The exception that caused the rollback</param>
        void Rollback(Exception exception);
    }
}