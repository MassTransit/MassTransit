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
namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using NUnit.Framework;
    using Saga;
    using TestFramework.Messages;


    [TestFixture, Explicit]
    public class SimpleConfiguration_Specs
    {
        [Test]
        public void FirstTestName()
        {
            IBusControl busControl = Bus.Factory.
                CreateUsingInMemory(x =>
            {
                x.UseTransform<PingMessage>(v =>
                {
                });

                x.UseConcurrencyLimit(3);
                x.UseRateLimit(1000);
                x.UseTransaction();

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Saga(new InMemorySagaRepository<SimpleSaga>(), s =>
                    {
                        s.UseConcurrentMessageLimit(1);
                        s.UseRateLimit(1000);
                    });

                    e.Consumer<MyConsumer>(c =>
                    {
                        c.UseConcurrentMessageLimit(1);
                        c.UseRateLimit(100);
                    });

                    e.UseTransaction();
                    e.UseConcurrencyLimit(7);
                    e.UseRateLimit(100);
                });
            });
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                throw new NotImplementedException();
            }
        }
    }
}