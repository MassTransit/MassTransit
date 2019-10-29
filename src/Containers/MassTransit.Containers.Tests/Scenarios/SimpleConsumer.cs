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
    using System.Threading.Tasks;
    using Internals.Extensions;


    public class SimpleConsumer :
        IConsumer<SimpleMessageInterface>
    {
        static readonly TaskCompletionSource<SimpleConsumer> _consumerCreated = TaskCompletionSourceFactory.New<SimpleConsumer>();

        readonly ISimpleConsumerDependency _dependency;
        readonly TaskCompletionSource<SimpleMessageInterface> _received;

        public SimpleConsumer(ISimpleConsumerDependency dependency)
        {
            _dependency = dependency;
            Console.WriteLine("SimpleConsumer()");

            _received = TaskCompletionSourceFactory.New<SimpleMessageInterface>();

            _consumerCreated.TrySetResult(this);
        }

        public Task<SimpleMessageInterface> Last
        {
            get { return _received.Task; }
        }

        public ISimpleConsumerDependency Dependency
        {
            get { return _dependency; }
        }

        public static Task<SimpleConsumer> LastConsumer
        {
            get { return _consumerCreated.Task; }
        }

        public async Task Consume(ConsumeContext<SimpleMessageInterface> message)
        {
            _dependency.DoSomething();

            _received.TrySetResult(message.Message);
        }
    }


    public class SimplerConsumer :
        IConsumer<SimpleMessageInterface>
    {
        static readonly TaskCompletionSource<SimplerConsumer> _consumerCreated = TaskCompletionSourceFactory.New<SimplerConsumer>();

        readonly TaskCompletionSource<SimpleMessageInterface> _received;

        public SimplerConsumer()
        {
            _received = TaskCompletionSourceFactory.New<SimpleMessageInterface>();

            _consumerCreated.TrySetResult(this);
        }

        public Task<SimpleMessageInterface> Last
        {
            get { return _received.Task; }
        }

        public static Task<SimplerConsumer> LastConsumer
        {
            get { return _consumerCreated.Task; }
        }

        public async Task Consume(ConsumeContext<SimpleMessageInterface> message)
        {
            _received.TrySetResult(message.Message);
        }
    }
}
