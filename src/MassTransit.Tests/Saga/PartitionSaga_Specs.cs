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
namespace MassTransit.Tests.Saga
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    [TestFixture]
    public class Partitioning_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_initiate_the_saga()
        {
            var timer = Stopwatch.StartNew();

            var ids = new Guid[Limit];
            for (int i = 0; i < Limit; i++)
            {
                var correlationId = NewId.NextGuid();
                await Bus.Publish(new CreateSaga {CorrelationId = correlationId});
                ids[i] = correlationId;
            }

            for (int i = 0; i < Limit; i++)
            {
                Guid? guid = await _repository.ShouldContainSaga(ids[i], TestTimeout);
                Assert.IsTrue(guid.HasValue);
            }

            timer.Stop();

            Console.WriteLine("Total time: {0}", timer.Elapsed);

            //Console.WriteLine(Bus.GetProbeResult().ToJsonString());
        }

        InMemorySagaRepository<LegacySaga> _repository;

        const int Limit = 100;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _repository = new InMemorySagaRepository<LegacySaga>();

            configurator.Saga(_repository, x =>
            {
                x.Message<CreateSaga>(m => m.UsePartitioner(4, p => p.Message.CorrelationId));
            });
        }


        class LegacySaga :
            ISaga,
            InitiatedBy<CreateSaga>
        {
            public Task Consume(ConsumeContext<CreateSaga> context)
            {
                return TaskUtil.Completed;
            }

            public Guid CorrelationId { get; set; }
        }


        class CreateSaga :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}