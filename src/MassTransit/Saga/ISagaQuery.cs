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
namespace MassTransit.Saga
{
    using System;
    using System.Linq.Expressions;


    /// <summary>
    /// A saga query is used when a LINQ expression is accepted to query
    /// the saga repository storage to get zero or more saga instances
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaQuery<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// The query expression that returns true if the saga
        /// matches the query.
        /// </summary>
        Expression<Func<TSaga, bool>> FilterExpression { get; }

        /// <summary>
        /// Compiles a function that can be used to programatically
        /// compare a saga instance to the filter expression.
        /// </summary>
        /// <returns></returns>
        Func<TSaga, bool> GetFilter();
    }
}