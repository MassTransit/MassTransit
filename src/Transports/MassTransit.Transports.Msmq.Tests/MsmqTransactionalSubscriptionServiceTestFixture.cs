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
    using Configuration;
    using MassTransit.Tests.TextFixtures;
    using NUnit.Framework;

    [TestFixture]
    public class MsmqTransactionalSubscriptionServiceTestFixture :
        SubscriptionServiceTestFixture<MsmqEndpoint>
    {
        protected override void EstablishContext()
        {
            SubscriptionServiceUri = "msmq://localhost/mt_subscriptions_tx";
            ClientControlUri = "msmq://localhost/mt_client_control_tx";
            ServerControlUri = "msmq://localhost/mt_server_control_tx";
            ClientUri = "msmq://localhost/mt_client_tx";
            ServerUri = "msmq://localhost/mt_server_tx";

            base.EstablishContext();
        }

        protected override void AdditionalEndpointFactoryConfiguration(IEndpointFactoryConfigurator x)
        {
            base.AdditionalEndpointFactoryConfiguration(x);

            MsmqEndpointConfigurator.Defaults(y =>
                {
                    y.CreateMissingQueues = true;
                    y.CreateTransactionalQueues = true;
                    y.PurgeOnStartup = true;
                });
        }
    }
}