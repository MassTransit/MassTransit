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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Util;


    class TestConsumePipe :
        IConsumePipe
    {
        readonly Func<ConsumeContext, Task> _callback;

        public TestConsumePipe(Func<ConsumeContext, Task> callback)
        {
            _callback = callback;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(ConsumeContext context)
        {
            await Task.Yield();

            await _callback(context);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectConsumeMessageObserver<TMessage>(IConsumeMessageObserver<TMessage> observer)
            where TMessage : class
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            throw new NotImplementedException();
        }

        public Task Connected => TaskUtil.Completed;
    }
}
