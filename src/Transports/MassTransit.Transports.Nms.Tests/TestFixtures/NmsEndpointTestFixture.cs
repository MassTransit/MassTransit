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
namespace MassTransit.Transports.Nms.Tests.TestFixtures
{
    using System;
    using Configuration;
    using Internal;
    using MassTransit.Tests.TextFixtures;
    using Rhino.Mocks;
    using Services.Subscriptions;

    public class NmsEndpointTestFixture :
        EndpointTestFixture<NmsEndpoint>
    {
        protected ISubscriptionService SubscriptionService { get; set; }
        protected IServiceBus LocalBus { get; set; }
        protected IServiceBus RemoteBus { get; set; }

        protected string ActiveMQHostName { get; set; }

        protected NmsEndpointTestFixture()
        {
            ActiveMQHostName = "192.168.0.195";
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            SetupSubscriptionService();

            LocalBus = ServiceBusConfigurator.New(x =>
                {
                    x.AddService<SubscriptionPublisher>();
                    x.AddService<SubscriptionConsumer>();
                    x.ReceiveFrom(new UriBuilder("activemq", ActiveMQHostName, 61616, "mt_client").Uri);
                });

            RemoteBus = ServiceBusConfigurator.New(x =>
                {
                    x.AddService<SubscriptionPublisher>();
                    x.AddService<SubscriptionConsumer>();
                    x.ReceiveFrom(new UriBuilder("activemq", ActiveMQHostName, 61616, "mt_server").Uri);
                });
        }

        private void SetupSubscriptionService()
        {
            SubscriptionService = new LocalSubscriptionService();
            ObjectBuilder.Stub(x => x.GetInstance<IEndpointSubscriptionEvent>())
                .Return(SubscriptionService);

            ObjectBuilder.Stub(x => x.GetInstance<SubscriptionPublisher>())
                .Return(null)
                .WhenCalled(invocation =>
                    {
                        // Return a unique instance of this class
                        invocation.ReturnValue = new SubscriptionPublisher(SubscriptionService);
                    });

            ObjectBuilder.Stub(x => x.GetInstance<SubscriptionConsumer>())
                .Return(null)
                .WhenCalled(invocation =>
                    {
                        // Return a unique instance of this class
                        invocation.ReturnValue = new SubscriptionConsumer(SubscriptionService, EndpointFactory);
                    });
        }


        protected override void TeardownContext()
        {
            LocalBus.Dispose();
            LocalBus = null;

            RemoteBus.Dispose();
            RemoteBus = null;

            SubscriptionService = null;

            base.TeardownContext();
        }
    }
}