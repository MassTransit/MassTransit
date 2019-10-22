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
namespace MassTransit.Tests.Pipeline
{
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.ConsumerFactories;
    using MassTransit.Testing;
    using TestFramework;
    using TestFramework.Messages;


    public abstract class MessageTestFixture :
        AsyncTestFixture
    {
        public MessageTestFixture()
            : base(new InMemoryTestHarness())
        {
        }

        protected ConsumeContext GetConsumeContext<T>(T message)
            where T : class
        {
            return new TestConsumeContext<T>(message);
        }

        protected OneMessageConsumer GetOneMessageConsumer()
        {
            return new OneMessageConsumer(GetTask<MessageA>());
        }

        protected TwoMessageConsumer GetTwoMessageConsumer()
        {
            return new TwoMessageConsumer(GetTask<MessageA>(), GetTask<MessageB>());
        }

        protected IConsumerFactory<T> GetInstanceConsumerFactory<T>(T consumer)
            where T : class
        {
            return new InstanceConsumerFactory<T>(consumer);
        }

        protected IConsumePipe CreateConsumePipe()
        {
            var builder = new ConsumePipeBuilder();

            return builder.Build();
        }
    }
}
