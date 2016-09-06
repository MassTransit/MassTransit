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
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using NUnit.Framework;
    using Pipeline.Pipes;
    using Transports.InMemory;
    using Util;


    [TestFixture]
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

        static readonly ILog _log = Logger.Get<InMemoryTestFixture>();

        IBusControl _bus;
        Uri _inputQueueAddress;
        ISendEndpoint _inputQueueSendEndpoint;
        ISendEndpoint _busSendEndpoint;
        BusHandle _busHandle;
        readonly Uri _baseAddress;
        InMemoryTransportCache _inMemoryTransportCache;
        readonly IBusCreationScope _busCreationScope;

        public Uri BaseAddress => _baseAddress;

        public InMemoryTestFixture(bool busPerTest = false)
        {
            _baseAddress = new Uri("loopback://localhost/");

            _inputQueueAddress = new Uri("loopback://localhost/input_queue");

            if (busPerTest)
                _busCreationScope = new PerTestBusCreationScope(SetupBus, TeardownBus);
            else
                _busCreationScope = new PerTestFixtureBusCreationScope(SetupBus, TeardownBus);
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => _inputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => _busSendEndpoint;

        protected Uri BusAddress => _bus.Address;

        protected Uri InputQueueAddress
        {
            get { return _inputQueueAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _inputQueueAddress = value;
            }
        }

        protected override IBus Bus => _bus;

        protected IRequestClient<TRequest, TResponse> CreateRequestClient<TRequest, TResponse>()
            where TRequest : class
            where TResponse : class
        {
            return Bus.CreateRequestClient<TRequest, TResponse>(InputQueueAddress, TestTimeout);
        }

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return _busCreationScope.TestFixtureSetup();
        }

        async Task SetupBus()
        {
            _bus = CreateBus();

            ConnectObservers(_bus);

            _busHandle = await _bus.StartAsync();

            _busSendEndpoint = await GetSendEndpoint(_bus.Address);

            _inputQueueSendEndpoint = await GetSendEndpoint(InputQueueAddress);
        }

        protected async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            var sendEndpoint = await _bus.GetSendEndpoint(address).ConfigureAwait(false);

            return sendEndpoint;
        }

        protected IPublishEndpointProvider PublishEndpointProvider => new InMemoryPublishEndpointProvider(Bus, _inMemoryTransportCache, PublishPipe.Empty);

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return _busCreationScope.TestFixtureTeardown();
        }

        async Task TeardownBus()
        {
            try
            {
                await _busHandle?.StopAsync(new CancellationTokenSource(TestTimeout).Token);
            }
            catch (Exception ex)
            {
                _log.Error("Bus Stop Failed: ", ex);
                throw;
            }
            finally
            {
                _busHandle = null;
                _bus = null;
            }
        }

        protected virtual void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConnectObservers(IBus bus)
        {
        }

        protected virtual void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingInMemory(x =>
            {
                _inMemoryTransportCache = new InMemoryTransportCache(Environment.ProcessorCount);

                x.SetTransportProvider(_inMemoryTransportCache);
                ConfigureBus(x);

                x.ReceiveEndpoint("input_queue", configurator => ConfigureInputQueueEndpoint(configurator));
            });
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