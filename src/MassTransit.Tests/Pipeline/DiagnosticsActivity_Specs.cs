// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
#if NETCOREAPP
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class DiagnosticsActivity_Specs :
        InMemoryTestFixture
    {
        readonly DiagnosticListener _listener;
        readonly IList<KeyValuePair<string, object>> _store;
        Task<ConsumeContext<PingMessage>> _handler;

        public DiagnosticsActivity_Specs()
        {
            _store = new List<KeyValuePair<string, object>>();
            _listener = new DiagnosticListener(GetType().FullName);
            _listener.Subscribe(new TestListener(_store));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);
            configurator.UseDiagnosticsActivity(_listener);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);
            _handler = Handled<PingMessage>(configurator);
        }

        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());
            await _handler;

            Assert.IsNotEmpty(_store);
        }


        class TestListener : IObserver<KeyValuePair<string, object>>
        {
            readonly IList<KeyValuePair<string, object>> _store;

            public TestListener(IList<KeyValuePair<string, object>> store)
            {
                _store = store;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(KeyValuePair<string, object> value)
            {
                _store.Add(value);
            }
        }
    }
}
#endif
