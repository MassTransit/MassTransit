// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using NUnit.Framework;
    using SubscriptionConfigurators;
    using MassTransit.Testing;


    [TestFixture]
    public class Sending_a_request_using_a_transactional_queue
    {
        [SetUp]
        public void Setup()
        {
            LocalBus = CreateBus("local", x => { });
            RemoteBus = CreateBus("remote", x => x.Consumer<SomeConsumer>());

            Assert.IsTrue(LocalBus.HasSubscription<InvalidRequest>().Any());
            Assert.IsTrue(LocalBus.HasSubscription<ValidRequest>().Any());

            CompleteEvent.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            LocalBus.Dispose();
            RemoteBus.Dispose();
            LocalBus = null;
            RemoteBus = null;
        }

        [Test]
        public void Should_receive_a_fault_from_a_failed_request()
        {
            bool responedHendeled = false;
            bool faultHendeled = false;

            CompleteEvent.WaitOne(TimeSpan.FromSeconds(2), true);

            LocalBus.PublishRequest(new InvalidRequest(), x =>
                {
                    x.Handle<Replay>(r =>
                        {
                            responedHendeled = true;
                            CompleteEvent.Set();
                        });
                    x.HandleFault((c, f) =>
                        {
                            faultHendeled = true;
                            CompleteEvent.Set();
                        });
                });

            CompleteEvent.WaitOne(Debugger.IsAttached ? 5.Minutes() : 20.Seconds(), true);

            Assert.That(responedHendeled, Is.False);
            Assert.That(faultHendeled, Is.True);
        }

        [Test]
        public void Should_receive_a_response_from_a_valid_request()
        {
            bool responseHandled = false;
            bool faultHandled = false;

            CompleteEvent.WaitOne(TimeSpan.FromSeconds(2), true);

            LocalBus.PublishRequest(new ValidRequest(), x =>
                {
                    x.Handle<Replay>(r =>
                        {
                            responseHandled = true;
                            CompleteEvent.Set();
                        });
                    x.HandleFault((c, f) =>
                        {
                            faultHandled = true;
                            CompleteEvent.Set();
                        });
                });

            CompleteEvent.WaitOne(TimeSpan.FromSeconds(20), true);

            Assert.That(responseHandled, Is.True);
            Assert.That(faultHandled, Is.False);
        }


        public class ValidRequest
        {
        }


        public class InvalidRequest
        {
        }


        public class Replay
        {
        }


        public IServiceBus LocalBus { get; set; }
        public IServiceBus RemoteBus { get; set; }
        static readonly ManualResetEvent CompleteEvent = new ManualResetEvent(false);

        IServiceBus CreateBus(string name, Action<SubscriptionBusServiceConfigurator> callback)
        {
            IServiceBus bus = ServiceBusFactory.New(sbc =>
                {
                    sbc.UseMsmq(x =>
                    {
                        x.UseMulticastSubscriptionClient();
                        x.VerifyMsmqConfiguration();
                    });
                    sbc.SetCreateTransactionalQueues(true);
                    sbc.ReceiveFrom("msmq://localhost/" + name);
                    sbc.UseJsonSerializer();
                    sbc.SetNetwork("Ran");

                    sbc.SetPurgeOnStartup(true);

                    sbc.Subscribe(callback);
                });

            return bus;
        }


        public class SomeConsumer :
            Consumes<ValidRequest>.Context,
            Consumes<InvalidRequest>.Context
        {
            public void Consume(IConsumeContext<InvalidRequest> message)
            {
                Debug.WriteLine("Request Handled");
                throw new InvalidOperationException("Some Invalid Request!");
            }

            public void Consume(IConsumeContext<ValidRequest> context)
            {
                Debug.WriteLine("Request Handled");
                context.Respond(new Replay());
            }
        }
    }

    [TestFixture]
    public class Sending_a_request_using_a_non_transactional_queue
    {
        [SetUp]
        public void Setup()
        {
            LocalBus = CreateBus("local_nontx", x => { });
            RemoteBus = CreateBus("remote_nontx", x => x.Consumer<SomeConsumer>());

            Assert.IsTrue(LocalBus.HasSubscription<InvalidRequest>().Any());
            Assert.IsTrue(LocalBus.HasSubscription<ValidRequest>().Any());

            CompleteEvent.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            LocalBus.Dispose();
            RemoteBus.Dispose();
            LocalBus = null;
            RemoteBus = null;
        }

        [Test]
        public void Should_receive_a_fault_from_a_failed_request()
        {
            bool responedHendeled = false;
            bool faultHendeled = false;

            CompleteEvent.WaitOne(TimeSpan.FromSeconds(2), true);

            LocalBus.PublishRequest(new InvalidRequest(), x =>
                {
                    x.Handle<Replay>(r =>
                        {
                            responedHendeled = true;
                            CompleteEvent.Set();
                        });
                    x.HandleFault((c, f) =>
                        {
                            faultHendeled = true;
                            CompleteEvent.Set();
                        });
                });

            CompleteEvent.WaitOne(Debugger.IsAttached ? 5.Minutes() : 20.Seconds(), true);

            Assert.That(responedHendeled, Is.False);
            Assert.That(faultHendeled, Is.True);
        }

        [Test]
        public void Should_receive_a_response_from_a_valid_request()
        {
            bool responseHandled = false;
            bool faultHandled = false;

            CompleteEvent.WaitOne(TimeSpan.FromSeconds(2), true);

            LocalBus.PublishRequest(new ValidRequest(), x =>
                {
                    x.Handle<Replay>(r =>
                        {
                            responseHandled = true;
                            CompleteEvent.Set();
                        });
                    x.HandleFault((c, f) =>
                        {
                            faultHandled = true;
                            CompleteEvent.Set();
                        });
                });

            CompleteEvent.WaitOne(TimeSpan.FromSeconds(20), true);

            Assert.That(responseHandled, Is.True);
            Assert.That(faultHandled, Is.False);
        }


        public class ValidRequest
        {
        }


        public class InvalidRequest
        {
        }


        public class Replay
        {
        }


        public IServiceBus LocalBus { get; set; }
        public IServiceBus RemoteBus { get; set; }
        static readonly ManualResetEvent CompleteEvent = new ManualResetEvent(false);

        IServiceBus CreateBus(string name, Action<SubscriptionBusServiceConfigurator> callback)
        {
            IServiceBus bus = ServiceBusFactory.New(sbc =>
                {
                    sbc.UseMsmq(x =>
                        {
                            x.UseMulticastSubscriptionClient();
                            x.VerifyMsmqConfiguration();
                        });
                    sbc.SetCreateTransactionalQueues(false);
                    sbc.ReceiveFrom("msmq://localhost/" + name);
                    sbc.UseJsonSerializer();
                    sbc.SetNetwork("RanDumb");

                    sbc.SetPurgeOnStartup(true);

                    sbc.Subscribe(callback);
                });

            return bus;
        }


        public class SomeConsumer :
            Consumes<ValidRequest>.Context,
            Consumes<InvalidRequest>.Context
        {
            public void Consume(IConsumeContext<InvalidRequest> message)
            {
                Debug.WriteLine("Request Handled");
                throw new InvalidOperationException("Some Invalid Request!");
            }

            public void Consume(IConsumeContext<ValidRequest> context)
            {
                Debug.WriteLine("Request Handled");
                context.Respond(new Replay());
            }
        }
    }
}