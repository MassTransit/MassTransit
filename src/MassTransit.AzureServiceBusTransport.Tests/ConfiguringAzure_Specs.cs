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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    namespace ConfiguringAzure_Specs
    {
        using System;
        using System.Threading.Tasks;
        using GreenPipes;
        using MassTransit.Testing;
        using Microsoft.ServiceBus;
        using Microsoft.ServiceBus.Messaging;
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class Configuring_a_bus_instance :
            AsyncTestFixture
        {
            [Test]
            public async Task Should_Support_NetMessaging_Protocol()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", "masstransit-build",
                    "MassTransit.AzureServiceBusTransport.Tests");

                var completed = new TaskCompletionSource<A>();

                IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    IServiceBusHost host = x.Host(serviceUri, h =>
                    {
                        h.SharedAccessSignature(s =>
                        {
                            s.KeyName = settings.KeyName;
                            s.SharedAccessKey = settings.SharedAccessKey;
                            s.TokenTimeToLive = settings.TokenTimeToLive;
                            s.TokenScope = settings.TokenScope;
                        });

                        h.TransportType = TransportType.NetMessaging;
                        h.OperationTimeout = TimeSpan.FromSeconds(30);
                        h.BatchFlushInterval = TimeSpan.FromMilliseconds(50);
                    });

                    x.ReceiveEndpoint(host, "input_queue", e =>
                    {
                        e.PrefetchCount = 16;

                        e.UseLog(Console.Out, async l => string.Format("Logging: {0}", l.Context.MessageId.Value));

                        e.Handler<A>(async context => completed.TrySetResult(context.Message));

                        // Add a message handler and configure the pipeline to retry the handler
                        // if an exception is thrown
                        e.Handler<A>(Handle, h =>
                        {
                            h.UseRetry(r => r.Interval(5, 100));
                        });
                    });
                });

                // TODO: Assert something here, need to get a hook to the underlying MessageReceiver
                BusHandle busHandle = await bus.StartAsync();

                await busHandle.StopAsync();
            }

            [Test]
            public async Task Should_support_the_new_syntax()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", "masstransit-build",
                    "MassTransit.AzureServiceBusTransport.Tests");

                var completed = new TaskCompletionSource<A>();

                IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    IServiceBusHost host = x.Host(serviceUri, h =>
                    {
                        h.SharedAccessSignature(s =>
                        {
                            s.KeyName = settings.KeyName;
                            s.SharedAccessKey = settings.SharedAccessKey;
                            s.TokenTimeToLive = settings.TokenTimeToLive;
                            s.TokenScope = settings.TokenScope;
                        });
                    });

                    x.ReceiveEndpoint(host, "input_queue", e =>
                    {
                        e.PrefetchCount = 16;

                        e.UseLog(Console.Out, async l => string.Format("Logging: {0}", l.Context.MessageId.Value));

                        e.Handler<A>(async context => completed.TrySetResult(context.Message));

                        // Add a message handler and configure the pipeline to retry the handler
                        // if an exception is thrown
                        e.Handler<A>(Handle, h =>
                        {
                            h.UseRetry(r => r.Interval(5, 100));
                        });
                    });
                });

                BusHandle busHandle = await bus.StartAsync();
                try
                {
                }
                finally
                {
                    await busHandle.StopAsync();
                }

                //                }))
                //                {
                //                    var queueAddress = new Uri(hostAddress, "input_queue");
                //                    ISendEndpoint endpoint = bus.GetSendEndpoint(queueAddress);
                //
                //                    await endpoint.Send(new A());
                //                }
            }

            [Test]
            public async Task Should_not_fail_when_slash_is_missing()
            {
                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", "masstransit-build",
                    "Test.Namespace");

               // serviceUri = new Uri(serviceUri.ToString().Trim('/'));

                IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    IServiceBusHost host = x.Host(serviceUri, h =>
                    {
                        h.SharedAccessSignature(s =>
                        {
                            s.KeyName = settings.KeyName;
                            s.SharedAccessKey = settings.SharedAccessKey;
                            s.TokenTimeToLive = settings.TokenTimeToLive;
                            s.TokenScope = settings.TokenScope;
                        });
                    });

                    x.ReceiveEndpoint(host, "test-queue", e =>
                    {
                        // Add a message handler and configure the pipeline to retry the handler
                        // if an exception is thrown
                        e.Handler<A>(Handle, h =>
                        {
                            h.UseRetry(r => r.Interval(5, 100));
                        });
                    });
                });

                BusHandle busHandle = await bus.StartAsync();
                try
                {
                }
                finally
                {
                    await busHandle.StopAsync();
                }
            }

            public Configuring_a_bus_instance()
                : base(new InMemoryTestHarness())
            {
            }

            async Task Handle(ConsumeContext<A> context)
            {
            }


            class A
            {
            }
        }
    }
}