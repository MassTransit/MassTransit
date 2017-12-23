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
namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using GreenPipes;
    using Pipeline.Filters;

    public class TransactionPipeSpecification<T> :
        ITransactionConfigurator,
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        IsolationLevel _isolationLevel;
        TimeSpan _timeout;

        public TransactionPipeSpecification()
        {
            _isolationLevel = IsolationLevel.ReadCommitted;
            _timeout = TimeSpan.FromSeconds(30);
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new TransactionFilter<T>(_isolationLevel, _timeout));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_timeout == TimeSpan.Zero)
                yield return this.Failure("Timeout", "Must be > 0");
        }

        public TimeSpan Timeout
        {
            set { _timeout = value; }
        }

        public IsolationLevel IsolationLevel
        {
            set { _isolationLevel = value; }
        }
    }
}