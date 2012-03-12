// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Linq.Expressions;
    using Saga;

    public class SimpleSaga :
        ISaga,
        InitiatedBy<FirstSagaMessage>,
        Orchestrates<SecondSagaMessage>,
        Observes<ThirdSagaMessage, SimpleSaga>
    {
        FirstSagaMessage _first;

        public SimpleSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected SimpleSaga()
        {
        }

        public FirstSagaMessage First
        {
            get { return _first; }
        }

        public void Consume(FirstSagaMessage message)
        {
            _first = message;
        }

        public Guid CorrelationId { get; private set; }

        public IServiceBus Bus { get; set; }

        public void Consume(ThirdSagaMessage message)
        {
        }

        public Expression<Func<SimpleSaga, ThirdSagaMessage, bool>> GetBindExpression()
        {
            return (saga, message) => saga.CorrelationId == message.CorrelationId;
        }

        public void Consume(SecondSagaMessage message)
        {
        }
    }
}