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
namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Pipeline.Sinks;
    using NUnit.Framework;


    [TestFixture]
    public abstract class AsyncTestFixture
    {
        protected TimeSpan TestTimeout;
        protected CancellationToken CancellationToken;

        protected AsyncTestFixture()
            : this(TimeSpan.FromSeconds(30))
        {
        }

        protected AsyncTestFixture(TimeSpan testTimeout)
        {
            TestTimeout = testTimeout;

            var cancellationTokenSource = new CancellationTokenSource(testTimeout);

            CancellationToken = cancellationTokenSource.Token;
        }

        protected TaskCompletionSource<T> GetTask<T>()
        {
            var source = new TaskCompletionSource<T>();
            CancellationToken.Register(() => source.TrySetCanceled());

            return source;
        }

        protected OneMessageConsumer GetOneMessageConsumer()
        {
            return new OneMessageConsumer(GetTask<MessageA>());
        }

        protected TwoMessageConsumer GetTwoMessageConsumer()
        {
            return new TwoMessageConsumer(GetTask<MessageA>(), GetTask<MessageB>());
        }

        protected IAsyncConsumerFactory<T> GetInstanceConsumerFactory<T>(T consumer)
            where T : class
        {
            return new InstanceAsyncConsumerFactory<T>(consumer);
        }
    }


    public class OneMessageConsumer :
        IConsumer<MessageA>
    {
        readonly TaskCompletionSource<MessageA> _completed;


        public OneMessageConsumer(TaskCompletionSource<MessageA> completed)
        {
            _completed = completed;
        }

        public Task<MessageA> Task
        {
            get { return _completed.Task; }
        }

        public async Task Consume(ConsumeContext<MessageA> context)
        {
            _completed.TrySetResult(context.Message);
        }
    }


    public class MessageA
    {
        
    }


    public class MessageB
    {
        
    }

    public class TwoMessageConsumer :
        IConsumer<MessageA>,
        IConsumer<MessageB>
    {
        readonly TaskCompletionSource<MessageA> _completed;
        readonly TaskCompletionSource<MessageB> _completed2;

        public TwoMessageConsumer(TaskCompletionSource<MessageA> completed, TaskCompletionSource<MessageB> completed2)
        {
            _completed = completed;
            _completed2 = completed2;
        }

        public Task<MessageA> TaskA
        {
            get { return _completed.Task; }
        }

        public Task<MessageB> TaskB
        {
            get { return _completed2.Task; }
        }

        async Task IConsumer<MessageA>.Consume(ConsumeContext<MessageA> context)
        {
            _completed.TrySetResult(context.Message);
        }

        async Task IConsumer<MessageB>.Consume(ConsumeContext<MessageB> context)
        {
            _completed2.TrySetResult(context.Message);
        }
    }
}