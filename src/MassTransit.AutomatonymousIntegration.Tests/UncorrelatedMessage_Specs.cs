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
namespace MassTransit.AutomatonymousIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using GreenPipes.Introspection;
    using NUnit.Framework;
    using Saga;
    using TestFramework;


    [TestFixture]
    public class When_a_message_is_not_correlated :
        InMemoryTestFixture
    {
        [Test, Explicit]
        public async Task Should_retry_the_status_message()
        {
            Task<Status> statusTask = null;
            Request<CheckStatus> request = await Bus.Request(InputQueueAddress, new CheckStatus("A"), x =>
            {
                statusTask = x.Handle<Status>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            await InputQueueSendEndpoint.Send(new Start("A", Guid.NewGuid()));

            Status status = await statusTask;

            Assert.AreEqual("A", status.ServiceName);
        }

        [Test, Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public async Task Should_start_and_handle_the_status_request()
        {
            Task<StartupComplete> startupCompletedTask = null;
            await Bus.Request(InputQueueAddress, new Start("A", Guid.NewGuid()), x =>
            {
                startupCompletedTask = x.Handle<StartupComplete>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            StartupComplete startupComplete = await startupCompletedTask;

            Task<Status> statusTask = null;
            await Bus.Request(InputQueueAddress, new CheckStatus("A"), x =>
            {
                statusTask = x.Handle<Status>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            Status status = await statusTask;

            Assert.AreEqual("A", status.ServiceName);
        }

        [Test]
        public async Task Should_start_and_handle_the_status_request_awaited()
        {
            Request<Start> request = await Bus.Request(InputQueueAddress, new Start("B", Guid.NewGuid()), x =>
            {
                x.Handle<StartupComplete>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            await request.Task;

            Task<Status> statusTask = null;
            await Bus.Request(InputQueueAddress, new CheckStatus("B"), x =>
            {
                statusTask = x.Handle<Status>();
                x.Timeout = TestTimeout;
            }, TestCancellationToken);

            Status status = await statusTask;

            Assert.AreEqual("B", status.ServiceName);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;


        class Instance :
            SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public string ServiceName { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started, x => x
                    .CorrelateBy(instance => instance.ServiceName, context => context.Message.ServiceName)
                    .SelectId(context => context.Message.ServiceId));
                Event(() => CheckStatus, x => x
                    .CorrelateBy(instance => instance.ServiceName, context => context.Message.ServiceName));

                Initially(
                    When(Started)
                        .Then(context => context.Instance.ServiceName = context.Data.ServiceName)
                        .Respond(context => new StartupComplete
                        {
                            ServiceId = context.Instance.CorrelationId,
                            ServiceName = context.Instance.ServiceName
                        })
                        .Then(context => Console.WriteLine("Started: {0} - {1}", context.Instance.CorrelationId, context.Instance.ServiceName))
                        .TransitionTo(Running));

                During(Running,
                    When(CheckStatus)
                        .Then(context => Console.WriteLine("Status check!"))
                        .Respond(context => new Status("Running", context.Instance.ServiceName)));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<CheckStatus> CheckStatus { get; private set; }
        }


        class Status
        {
            public Status(string status, string serviceName)
            {
                StatusDescription = status;
                ServiceName = serviceName;
            }

            public string ServiceName { get; set; }
            public string StatusDescription { get; set; }
        }


        class CheckStatus
        {
            public CheckStatus(string serviceName)
            {
                ServiceName = serviceName;
            }

            public CheckStatus()
            {
            }

            public string ServiceName { get; set; }
        }


        class Start
        {
            public Start(string serviceName, Guid serviceId)
            {
                ServiceName = serviceName;
                ServiceId = serviceId;
            }

            public Start()
            {
            }

            public string ServiceName { get; set; }
            public Guid ServiceId { get; set; }
        }


        class StartupComplete
        {
            public Guid ServiceId { get; set; }
            public string ServiceName { get; set; }
        }
    }
}