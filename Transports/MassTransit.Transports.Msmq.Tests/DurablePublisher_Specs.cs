// Copyright 2007-2008 The Apache Software Foundation.
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
    using System.Threading;
    using Configuration;
    using Magnum.Actors;
    using Magnum.DateTimeExtensions;
    using MassTransit.Tests;
    using MassTransit.Tests.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class MsmqDurablePublisherTestFixture : 
        MsmqTransactionalSubscriptionServiceTestFixture
    {
        protected override void AdditionalEndpointFactoryConfiguration(IEndpointFactoryConfigurator x)
        {
            base.AdditionalEndpointFactoryConfiguration(x);

            MsmqEndpointConfigurator.Defaults(y => { y.RemoteRelayQueue = new Uri("msmq://localhost/mt_remote_relay_tx"); });
        }

    }

    [TestFixture]
    public class Publishing_a_message_using_the_durable_publisher :
        MsmqDurablePublisherTestFixture
    {
        [Test]
        public void Should_deliver_the_message_once_the_endpoint_is_available()
        {
            var received = new Future<PingMessage>();

            using (RemoteBus.Subscribe<PingMessage>(received.Complete).Disposable())
            {
                Thread.Sleep(100);

                var ping = new PingMessage();
                LocalBus.Publish(ping);

                received.IsAvailable(5.Seconds()).ShouldBeTrue();
            }
        }
    }
}