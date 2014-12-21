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
namespace MassTransit.Saga
{
    using System;
    using System.Linq.Expressions;


    /// <summary>
    /// A saga filter includes both a filter expression and the compiled lambda method
    /// used to locate a saga instance.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public interface ISagaFilter<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Compiled on-demand to filter the sagas in-memory
        /// </summary>
        Func<TSaga, bool> Filter { get; }

        /// <summary>
        /// Can be used to filter for sagas based on an expression (using with LINQ to SQL, query-over, etc.)
        /// </summary>
        Expression<Func<TSaga, bool>> FilterExpression { get; }
    }
}