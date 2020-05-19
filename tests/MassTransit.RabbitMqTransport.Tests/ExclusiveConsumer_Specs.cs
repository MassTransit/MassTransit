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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes.Internals.Extensions;
    using NUnit.Framework;
    using RabbitMqTransport.Testing;
    using TestFramework.Logging;


    public class ExclusiveConsumer_Specs :
        RabbitMqTestFixture
    {
        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ExclusiveConsumer = true;
        }

        [Test]
        public async Task Should_not_be_allowed_twice()
        {
            var loggerFactory = new TestOutputLoggerFactory(true);

            LogContext.ConfigureCurrentLogContext(loggerFactory);

            DiagnosticListener.AllListeners.Subscribe(new DiagnosticListenerObserver());

            var secondHarness = new RabbitMqTestHarness();

            try
            {
                Assert.That(async () =>
                {
                    using (var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                    {
                        await secondHarness.Start(token.Token).OrCanceled(TestCancellationToken);

                        await secondHarness.Stop();
                    }
                }, Throws.TypeOf<RabbitMqConnectionException>());
            }
            finally
            {
                secondHarness.Dispose();
            }
        }
    }
}
