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
namespace MassTransit.TestFramework
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using NUnit.Framework;
    using Transports.InMemory;


    [TestFixture]
    public class InMemoryTestFixture :
        BusTestFixture
    {
        static readonly ILog _log = Logger.Get<InMemoryTestFixture>();

        IBusControl _bus;
        Uri _inputQueueAddress;
        ISendEndpoint _inputQueueSendEndpoint;
        ISendEndpoint _busSendEndpoint;
        BusHandle _busHandle;
        readonly Uri _baseAddress;
        InMemoryTransportCache _inMemoryTransportCache;

        public Uri BaseAddress
        {
            get { return _baseAddress; }
        }

        public InMemoryTestFixture()
        {
            _baseAddress = new Uri("loopback://localhost/");

            _inputQueueAddress = new Uri("loopback://localhost/input_queue");
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint
        {
            get { return _inputQueueSendEndpoint; }
        }

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint
        {
            get { return _busSendEndpoint; }
        }

        protected Uri BusAddress
        {
            get { return _bus.Address; }
        }

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

        protected override IBus Bus
        {
            get { return _bus; }
        }

        protected IRequestClient<TRequest, TResponse> CreateRequestClient<TRequest, TResponse>()
            where TRequest : class
            where TResponse : class
        {
            return Bus.CreateRequestClient<TRequest, TResponse>(InputQueueAddress, TestTimeout);
        }

        [TestFixtureSetUp]
        public void SetupInMemoryTestFixture()
        {
            _bus = CreateBus();

            _busHandle = _bus.Start();

            _busSendEndpoint = Await(() => GetSendEndpoint(_bus.Address));

            _inputQueueSendEndpoint = Await(() => GetSendEndpoint(InputQueueAddress));
        }

        protected async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            ISendEndpoint sendEndpoint = await _bus.GetSendEndpoint(address);

            return sendEndpoint;
        }

        protected IPublishSendEndpointProvider PublishSendEndpointProvider
        {
            get { return new InMemoryPublishSendEndpointProvider(Bus, _inMemoryTransportCache); }
        }

        [TestFixtureTearDown]
        public void TearDownInMemoryTestFixture()
        {
            try
            {
                if (_busHandle != null)
                    Await(() => _busHandle.Stop(new CancellationTokenSource(TestTimeout).Token));
            }
            catch (AggregateException ex)
            {
                _log.Error("LocalBus Stop Failed: ", ex);
                throw;
            }

            _bus = null;
        }

        protected virtual void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingInMemory(x =>
            {
                _inMemoryTransportCache = new InMemoryTransportCache(Environment.ProcessorCount);

                x.SetTransportProvider(_inMemoryTransportCache);
                ConfigureBus(x);

                x.ReceiveEndpoint("input_queue", ConfigureInputQueueEndpoint);
            });
        }
    }
}