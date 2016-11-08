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
    using Events;
    using Saga;


    public class RescueExceptionSagaConsumeContext<TSaga> :
        ConsumeContextProxy,
        ExceptionSagaConsumeContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContext<TSaga> _context;
        readonly Exception _exception;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionSagaConsumeContext(SagaConsumeContext<TSaga> context, Exception exception)
            : base(context)
        {
            _context = context;
            _exception = exception;
        }

        public TSaga Saga => _context.Saga;

        public SagaConsumeContext<TSaga, T> PopContext<T>() where T : class
        {
            return _context.PopContext<T>();
        }

        public Task SetCompleted()
        {
            return _context.SetCompleted();
        }

        public bool IsCompleted => _context.IsCompleted;

        Exception ExceptionSagaConsumeContext<TSaga>.Exception => _exception;

        ExceptionInfo ExceptionSagaConsumeContext<TSaga>.ExceptionInfo
        {
            get
            {
                if (_exceptionInfo != null)
                    return _exceptionInfo;

                _exceptionInfo = new FaultExceptionInfo(_exception);

                return _exceptionInfo;
            }
        }
    }
}