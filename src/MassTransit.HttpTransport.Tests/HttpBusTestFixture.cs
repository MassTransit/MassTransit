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
namespace MassTransit.HttpTransport.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using NUnit.Framework;
    using TestFramework;
    using Testing.TestDecorators;


    public class HttpBusTestFixture :
        BusTestFixture
    {
        static readonly ILog _log = Logger.Get<HttpBusTestFixture>();
        IBusControl _bus;
        BusHandle _busHandle;
        ISendEndpoint _inputQueueSendEndpoint;
        //ISendEndpoint _busSendEndpoint;
        readonly TestSendObserver _sendObserver;
        Uri _hostAddress;

        public HttpBusTestFixture()
        {
            _sendObserver = new TestSendObserver(TestTimeout);
            _hostAddress = new Uri("http://localhost:8080");
        }


        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => _inputQueueSendEndpoint;
        protected override IBus Bus => _bus;

        protected Uri InputQueueAddress
        {
            get { return _hostAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _hostAddress = value;
            }
        }

        [OneTimeSetUp]
        public async Task SetupInMemoryTestFixture()
        {
             _bus = CreateBus();

            _busHandle = await _bus.StartAsync();
            try
            {
//                _busSendEndpoint = Await(() => _bus.GetSendEndpoint(_bus.Address));
//                _busSendEndpoint.ConnectSendObserver(_sendObserver);

                _inputQueueSendEndpoint = Await(() => _bus.GetSendEndpoint(_hostAddress));
                _inputQueueSendEndpoint.ConnectSendObserver(_sendObserver);
            }
            catch (Exception)
            {
                try
                {
                    using (var tokenSource = new CancellationTokenSource(TestTimeout))
                    {
                        await _busHandle?.StopAsync(tokenSource.Token);
                    }
                }
                finally
                {
                    _busHandle = null;
                    _bus = null;
                }

                throw;
            }
        }

        [OneTimeTearDown]
        public async Task TearDownInMemoryTestFixture()
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(TestTimeout))
                {
                    await _bus.StopAsync(tokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Bus Stop Failed", ex);
            }
            finally
            {
                _busHandle = null;
                _bus = null;
            }
        }

        protected virtual void ConfigureBus(IHttpBusFactoryConfigurator configurator)
        {
            Host = configurator.Host(_hostAddress);
        }

        protected IHttpHost Host { get; private set; }

        protected virtual void ConfigureInputQueueEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
        }


        IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingHttp(x =>
            {
                ConfigureBus(x);

                x.ReceiveEndpoint(Host, ConfigureInputQueueEndpoint);
            });
        }
    }
}