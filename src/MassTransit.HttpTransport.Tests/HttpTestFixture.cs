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
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class HttpTestFixture :
        BusTestFixture
    {
        static readonly ILog _log = Logger.Get<HttpTestFixture>();

        public HttpTestFixture()
        {
            HttpTestHarness = new HttpTestHarness(new Uri("http://localhost:8080"));

            HttpTestHarness.OnConfigureBus += ConfigureBus;
            HttpTestHarness.OnConfigureBusHost += ConfigureBusHost;
            HttpTestHarness.OnConfigureRootReceiveEndpoint += ConfigureRootReceiveEndpoint;
        }

        protected HttpTestHarness HttpTestHarness { get; }

        protected override BusTestHarness BusTestHarness => HttpTestHarness;

        protected Uri HostAddress => HttpTestHarness.HostAddress;

        protected ISendEndpoint RootEndpoint => HttpTestHarness.InputQueueSendEndpoint;

        protected IHttpHost Host => HttpTestHarness.Host;

        [OneTimeSetUp]
        public Task SetupHttpTestFixture()
        {
            return HttpTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownHttpTestFixture()
        {
            return HttpTestHarness.Stop();
        }

        protected virtual void ConfigureBus(IHttpBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureBusHost(IHttpBusFactoryConfigurator configurator, IHttpHost host)
        {
        }

        protected virtual void ConfigureRootReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
        }
    }
}