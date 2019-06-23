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
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class HttpTestFixture :
        BusTestFixture
    {
        public HttpTestFixture()
            : this(new HttpTestHarness(new Uri("http://localhost:8080")))
        {
        }

        public HttpTestFixture(HttpTestHarness harness)
            : base(harness)
        {
            HttpTestHarness = harness;

            HttpTestHarness.OnConfigureHttpBus += ConfigureHttpBus;
            HttpTestHarness.OnConfigureHttpBusHost += ConfigureHttpBusHost;
            HttpTestHarness.OnConfigureHttpReceiveEndpoint += ConfigureHttpReceiveEndpoint;
        }

        protected HttpTestHarness HttpTestHarness { get; }

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

        protected virtual void ConfigureHttpBus(IHttpBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureHttpBusHost(IHttpBusFactoryConfigurator configurator, IHttpHost host)
        {
        }

        protected virtual void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
        }
    }
}
