// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Tests.TurningItOut
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Configuration;
    using MassTransit.Saga;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    [Explicit]
    public class A_successful_job_using_turnout :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_result_in_complete_state_instance()
        {
            var endpoint = await Bus.GetSendEndpoint(_serviceAddress);

            await endpoint.Send(new LongTimeJob
            {
                Id = "FIRST",
                Duration = TimeSpan.FromSeconds(10)
            });

            ConsumeContext<TurnoutJobCompleted> consumeContext = await _handler;
        }

        Uri _serviceAddress;
        ISagaRepository<TurnoutJobState> _repository;
        TurnoutJobStateMachine _stateMachine;
        Task<ConsumeContext<TurnoutJobCompleted>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<TurnoutJobCompleted>(configurator);
        }

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            configurator.UseServiceBusMessageScheduler();

            base.ConfigureServiceBusBusHost(configurator, host);

            configurator.TurnoutEndpoint<LongTimeJob>(host, "service_queue", endpoint =>
            {
                endpoint.SuperviseInterval = TimeSpan.FromSeconds(1);
                endpoint.SetJobFactory(async context =>
                {
                    Console.WriteLine("Starting job: {0}", context.Command.Id);

                    await Task.Delay(context.Command.Duration).ConfigureAwait(false);

                    Console.WriteLine("Finished job: {0}", context.Command.Id);
                });

                _serviceAddress = endpoint.InputAddress;
            });

            _stateMachine = new TurnoutJobStateMachine();
            _repository = new MessageSessionSagaRepository<TurnoutJobState>();


            configurator.ReceiveEndpoint(host, "service_state", endpoint =>
            {
                endpoint.RequiresSession = true;
                endpoint.MessageWaitTimeout = TimeSpan.FromHours(8);

                endpoint.StateMachineSaga(_stateMachine, _repository);
            });
        }
    }
}