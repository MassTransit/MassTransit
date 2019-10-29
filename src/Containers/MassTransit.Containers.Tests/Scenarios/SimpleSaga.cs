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
namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using Saga;


    public class SimpleSaga :
        ISaga,
        InitiatedBy<FirstSagaMessage>,
        Orchestrates<SecondSagaMessage>,
        Observes<ThirdSagaMessage, SimpleSaga>
    {
        readonly TaskCompletionSource<FirstSagaMessage> _first = TaskCompletionSourceFactory.New<FirstSagaMessage>();
        readonly TaskCompletionSource<SecondSagaMessage> _second = TaskCompletionSourceFactory.New<SecondSagaMessage>();
        readonly TaskCompletionSource<ThirdSagaMessage> _third = TaskCompletionSourceFactory.New<ThirdSagaMessage>();

        public SimpleSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected SimpleSaga()
        {
        }

        public Task<FirstSagaMessage> First
        {
            get { return _first.Task; }
        }

        public Task<SecondSagaMessage> Second
        {
            get { return _second.Task; }
        }

        public Task<ThirdSagaMessage> Third
        {
            get { return _third.Task; }
        }

        public async Task Consume(ConsumeContext<FirstSagaMessage> context)
        {
            Console.WriteLine("SimpleSaga: First: {0}", context.Message.CorrelationId);
            _first.TrySetResult(context.Message);
        }

        public Guid CorrelationId { get; set; }

        public async Task Consume(ConsumeContext<ThirdSagaMessage> context)
        {
            Console.WriteLine("SimpleSaga: Third: {0}", context.Message.CorrelationId);
            _third.TrySetResult(context.Message);
        }

        Expression<Func<SimpleSaga, ThirdSagaMessage, bool>> Observes<ThirdSagaMessage, SimpleSaga>.CorrelationExpression
        {
            get { return (saga, message) => saga.CorrelationId == message.CorrelationId; }
        }

        public async Task Consume(ConsumeContext<SecondSagaMessage> context)
        {
            Console.WriteLine("SimpleSaga: Second: {0}", context.Message.CorrelationId);
            _second.TrySetResult(context.Message);
        }
    }
}
