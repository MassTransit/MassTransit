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
namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Automatonymous;
    using Lamar;
    using LamarIntegration;
    using NUnit.Framework;
    using Saga;
    using Scenarios.StateMachines;
    using TestFramework;


    public class LamarStateMachineSaga_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_first_event_successfully()
        {
            Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();
            Task<ConsumeContext<TestUpdated>> updated = ConnectPublishHandler<TestUpdated>();

            await InputQueueSendEndpoint.Send(new StartTest {CorrelationId = NewId.NextGuid(), TestKey = "Unique"});

            await started;

            await InputQueueSendEndpoint.Send(new UpdateTest {TestKey = "Unique"});

            await updated;
        }

        IContainer _container;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _container = new Container(x =>
            {
                x.For(typeof(ISagaRepository<>)).Use(typeof(InMemorySagaRepository<>)).Singleton();

                x.For<TestStateMachineSaga>().Use<TestStateMachineSaga>().Singleton();
                x.For(typeof(SagaStateMachine<TestInstance>)).Use(typeof(TestStateMachineSaga)).Singleton();
            });

            configurator.LoadStateMachineSagas(_container);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        ///// <summary>
        ///// All requests For the "TO" types will be filled by fetching the "FROM"
        ///// type and casting it to "TO"
        ///// GetInstance(typeof(TO)) basically becomes (TO)GetInstance(typeof(FROM))
        ///// </summary>
        ///// <typeparam name="TFrom"></typeparam>
        ///// <typeparam name="TTo"></typeparam>
        //public void Forward<TFrom, TTo>() where TFrom : class where TTo : class
        //{
        //    this.For<TTo>((ILifecycle)null).AddInstances((Action<IInstanceExpression<TTo>>)(x => x.ConstructedBy<TTo>((Expression<Func<IContext, TTo>>)(c => c.GetInstance<TFrom>() as TTo))));
        //}
    }
}
