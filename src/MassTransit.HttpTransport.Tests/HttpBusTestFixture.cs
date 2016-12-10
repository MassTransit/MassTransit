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
    using System.Net.Http;
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

        readonly TestSendObserver _sendObserver;
        IBusControl _bus;
        BusHandle _busHandle;
        Uri _hostAddress;
        ISendEndpoint _rootEndpoint;

        public HttpBusTestFixture()
        {
            _sendObserver = new TestSendObserver(TestTimeout);
            _hostAddress = new Uri("http://localhost:8080");
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint RootEndpoint => _rootEndpoint;

        protected override IBus Bus => _bus;

        protected Uri HostAddress
        {
            get { return _hostAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The Bus has already been created, too late to change the address");

                _hostAddress = value;
            }
        }

        protected IHttpHost Host { get; private set; }

        [OneTimeSetUp]
        public async Task SetupHttpTestFixture()
        {
            _bus = CreateBus();

            _busHandle = await _bus.StartAsync(TestCancellationToken);
            try
            {
                _rootEndpoint = Await(() => _bus.GetSendEndpoint(_hostAddress));
                _rootEndpoint.ConnectSendObserver(_sendObserver);
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
        public async Task TearDownHttpTestFixture()
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

                throw;
            }
            finally
            {
                _busHandle = null;
                _bus = null;
            }
        }

        protected virtual void ConfigureBus(IHttpBusFactoryConfigurator configurator)
        {
            Host = configurator.Host(_hostAddress, h => h.Method = HttpMethod.Post);
        }

        protected virtual void ConfigureBusHost(IHttpBusFactoryConfigurator configurator, IHttpHost host)
        {
        }

        protected virtual void ConfigureRootReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingHttp(x =>
            {
                ConfigureBus(x);

                ConfigureBusHost(x, Host);

                x.ReceiveEndpoint(Host, "", ConfigureRootReceiveEndpoint);
            });
        }
    }
}