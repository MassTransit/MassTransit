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
namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Saga;
    using Util;


    public class RetrySagaConsumeContext<TSaga> :
        RetryConsumeContext,
        SagaConsumeContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContext<TSaga> _context;

        public RetrySagaConsumeContext(SagaConsumeContext<TSaga> context)
            : base(context)
        {
            _context = context;
        }

        public TSaga Saga => _context.Saga;

        SagaConsumeContext<TSaga, T> SagaConsumeContext<TSaga>.PopContext<T>()
        {
            return _context.PopContext<T>();
        }

        Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            return _context.SetCompleted();
        }

        public bool IsCompleted => _context.IsCompleted;

        public override TContext CreateNext<TContext>()
        {
            return new RetrySagaConsumeContext<TSaga>(_context) as TContext
                ?? throw new ArgumentException($"The context type is not valid: {TypeMetadataCache<TContext>.ShortName}");
        }
    }
}