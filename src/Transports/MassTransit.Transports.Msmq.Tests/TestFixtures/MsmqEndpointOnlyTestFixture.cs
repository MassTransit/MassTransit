// Copyright 2007-2011 The Apache Software Foundation.
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
	using Configurators;
	using EndpointConfigurators;
	using MassTransit.Tests.TextFixtures;

    public class MsmqEndpointOnlyTestFixture :
        EndpointTestFixture<MsmqTransportFactory>
    {
        public MsmqEndpointOnlyTestFixture() :
            this(new CreateEndpointSettings("msmq://localhost/mt_client"))
        {
        }

        public MsmqEndpointOnlyTestFixture(CreateEndpointSettings settings)
        {
            Transactional = settings.Transactional;
            EndpointAddress = settings.Address;
            ErrorEndpointAddress = settings.ErrorAddress;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            EndpointConfiguratorImpl.Defaults(x =>
            {
                x.CreateMissingQueues = true;
                x.CreateTransactionalQueues = Transactional;
                x.PurgeOnStartup = true;
            });

            Endpoint = EndpointCache.GetEndpoint(EndpointAddress.Uri);
            ErrorEndpoint = EndpointCache.GetEndpoint(ErrorEndpointAddress.Uri);
        }

        protected override void TeardownContext()
        {
            Endpoint = null;
            ErrorEndpoint = null;

            base.TeardownContext();
        }

        protected bool Transactional { get; private set; }
        protected IEndpointAddress EndpointAddress { get; set; }
        protected IEndpointAddress ErrorEndpointAddress { get; set; }
        protected IEndpoint Endpoint { get; private set; }
        protected IEndpoint ErrorEndpoint { get; private set; }
    }
}