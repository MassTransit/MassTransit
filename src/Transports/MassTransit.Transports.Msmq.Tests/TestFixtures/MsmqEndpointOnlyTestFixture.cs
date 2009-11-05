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
namespace MassTransit.Transports.Msmq.Tests.TestFixtures
{
    using System;
    using MassTransit.Tests.TextFixtures;

    public class MsmqEndpointOnlyTestFixture :
        EndpointTestFixture<MsmqEndpoint>
    {
        protected bool Transactional { get; set; }

        public MsmqEndpointOnlyTestFixture()
        {
        	var settings = new CreateMsmqEndpointSettings(new Uri("msmq://localhost/mt_client"));

        	EndpointAddress = settings.Address;
            ErrorEndpointAddress = settings.ErrorAddress;
            Transactional = false;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            MsmqEndpointConfigurator.Defaults(x =>
                {
                    x.CreateMissingQueues = true;
                    x.CreateTransactionalQueues = Transactional;
                    x.PurgeOnStartup = true;
                });

            Endpoint = EndpointFactory.GetEndpoint(EndpointAddress.Uri);
            ErrorEndpoint = EndpointFactory.GetEndpoint(ErrorEndpointAddress.Uri);
        }

        protected override void TeardownContext()
        {
            Endpoint = null;
            ErrorEndpoint = null;

            base.TeardownContext();
        }

        protected IMsmqEndpointAddress EndpointAddress { get; set; }
        protected IMsmqEndpointAddress ErrorEndpointAddress { get; set; }
        protected IEndpoint Endpoint { get; private set; }
        protected IEndpoint ErrorEndpoint { get; private set; }
    }
}