// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Grid.Tests
{
    using System;
    using System.Threading;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Tests;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using ServiceBus;
    using WindsorIntegration;

    [TestFixture]
    public class As_a_developer_that_needs_to_distribute_a_task_across_multiple_servers_for_parallel_processing :
        Specification
    {
        private readonly DefaultMassTransitContainer _container = new DefaultMassTransitContainer("castle.xml");
        private IServiceBus _bus;
        private IObjectBuilder _builder;
        private IEndpointResolver _endpointResolver;

        protected override void Before_each()
        {
            _bus = _container.Resolve<IServiceBus>();
            _builder = _container.Resolve<IObjectBuilder>();
            _endpointResolver = _container.Resolve<IEndpointResolver>();

            _container.AddComponent<FactorLongNumberWorker>();
        }

        [Test]
        public void I_want_to_be_able_to_define_a_distributed_task_and_have_it_processed()
        {
            _bus.AddComponent<SubTaskWorker<FactorLongNumberWorker, FactorLongNumber, LongNumberFactored>>();

            FactorLongNumbers factorLongNumbers = new FactorLongNumbers();

            Random random = new Random();

            for (int i = 0; i < 2; i++)
            {
                long value = (long) (random.NextDouble()*1000000);

                factorLongNumbers.Add(value);
            }

            ManualResetEvent _complete = new ManualResetEvent(false);

            factorLongNumbers.WhenComplete(x => _complete.Set());

            DistributedTask<FactorLongNumbers, FactorLongNumber, LongNumberFactored> distributedTask =
                new DistributedTask<FactorLongNumbers, FactorLongNumber, LongNumberFactored>(_bus, _endpointResolver, factorLongNumbers);

            distributedTask.Start();

            Assert.That(_complete.WaitOne(TimeSpan.FromMinutes(1), true), Is.True, "Timeout waiting for distributed task to complete");
        }
    }
}