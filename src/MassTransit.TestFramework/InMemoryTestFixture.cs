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
namespace MassTransit.TestFramework
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Testing;
    using Transports.InMemory;
    using Util;


    public class InMemoryTestFixture :
        BusTestFixture
    {
        [SetUp]
        public Task SetupInMemoryTest()
        {
            return _busCreationScope.TestSetup();
        }

        [TearDown]
        public Task TearDownInMemoryTest()
        {
            return _busCreationScope.TestTeardown();
        }

        protected InMemoryTestHarness InMemoryTestHarness { get; }

        readonly IBusCreationScope _busCreationScope;

        protected string InputQueueName => InMemoryTestHarness.InputQueueName;

        protected Uri BaseAddress => InMemoryTestHarness.BaseAddress;

        protected IInMemoryHost Host => InMemoryTestHarness.Host;

        public InMemoryTestFixture(bool busPerTest = false)
            : this(new InMemoryTestHarness(), busPerTest)
        {
        }

        public InMemoryTestFixture(InMemoryTestHarness harness, bool busPerTest = false)
            : base(harness)
        {
            InMemoryTestHarness = harness;

            if (busPerTest)
                _busCreationScope = new PerTestBusCreationScope(SetupBus, TeardownBus);
            else
                _busCreationScope = new PerTestFixtureBusCreationScope(SetupBus, TeardownBus);

            InMemoryTestHarness.OnConfigureInMemoryBus += ConfigureInMemoryBus;
            InMemoryTestHarness.OnConfigureInMemoryReceiveEndpoint += ConfigureInMemoryReceiveEndpoint;
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => InMemoryTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => InMemoryTestHarness.BusSendEndpoint;

        protected Uri BusAddress => InMemoryTestHarness.BusAddress;

        protected Uri InputQueueAddress => InMemoryTestHarness.InputQueueAddress;

        protected IRequestClient<TRequest, TResponse> CreateRequestClient<TRequest, TResponse>()
            where TRequest : class
            where TResponse : class
        {
            return InMemoryTestHarness.CreateRequestClient<TRequest, TResponse>();
        }

        protected IRequestClient<TRequest> CreateRequestClient<TRequest>()
            where TRequest : class
        {
            return InMemoryTestHarness.CreateRequestClient<TRequest>();
        }

        protected Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>()
            where TRequest : class
        {
            return InMemoryTestHarness.ConnectRequestClient<TRequest>();
        }

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return _busCreationScope.TestFixtureSetup();
        }

        Task SetupBus()
        {
            return InMemoryTestHarness.Start();
        }

        protected Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return InMemoryTestHarness.GetSendEndpoint(address);
        }

        [OneTimeTearDown]
        public async Task TearDownInMemoryTestFixture()
        {
            await _busCreationScope.TestFixtureTeardown().ConfigureAwait(false);

            InMemoryTestHarness.Dispose();
        }

        Task TeardownBus()
        {
            return InMemoryTestHarness.Stop();
        }

        protected virtual void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
        }

        interface IBusCreationScope
        {
            Task TestFixtureSetup();
            Task TestSetup();
            Task TestTeardown();
            Task TestFixtureTeardown();
        }


        class PerTestFixtureBusCreationScope :
            IBusCreationScope
        {
            readonly Func<Task> _setupBus;
            readonly Func<Task> _teardownBus;

            public PerTestFixtureBusCreationScope(Func<Task> setupBus, Func<Task> teardownBus)
            {
                _setupBus = setupBus;
                _teardownBus = teardownBus;
            }

            public Task TestFixtureSetup()
            {
                return _setupBus();
            }

            public Task TestSetup()
            {
                return TaskUtil.Completed;
            }

            public Task TestTeardown()
            {
                return TaskUtil.Completed;
            }

            public Task TestFixtureTeardown()
            {
                return _teardownBus();
            }
        }


        class PerTestBusCreationScope :
            IBusCreationScope
        {
            readonly Func<Task> _setupBus;
            readonly Func<Task> _teardownBus;

            public PerTestBusCreationScope(Func<Task> setupBus, Func<Task> teardownBus)
            {
                _setupBus = setupBus;
                _teardownBus = teardownBus;
            }

            public Task TestFixtureSetup()
            {
                return TaskUtil.Completed;
            }

            public Task TestSetup()
            {
                return _setupBus();
            }

            public Task TestTeardown()
            {
                return _teardownBus();
            }

            public Task TestFixtureTeardown()
            {
                return TaskUtil.Completed;
            }
        }
    }
}
