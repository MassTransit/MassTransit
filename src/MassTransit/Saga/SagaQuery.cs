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


    public class SagaQuery<TSaga> :
        ISagaQuery<TSaga>
        where TSaga : class, ISaga
    {
        readonly Lazy<Func<TSaga, bool>> _filter;
        readonly Expression<Func<TSaga, bool>> _filterExpression;

        public SagaQuery(Expression<Func<TSaga, bool>> filterExpression)
        {
            _filterExpression = filterExpression;
            _filter = new Lazy<Func<TSaga, bool>>(filterExpression.Compile);
        }

        public Func<TSaga, bool> GetFilter()
        {
            return _filter.Value;
        }

        public Expression<Func<TSaga, bool>> FilterExpression
        {
            get { return _filterExpression; }
        }
    }
}